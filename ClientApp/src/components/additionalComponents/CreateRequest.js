import React, { Component } from 'react';
import {Button, Col, Container, Row} from "reactstrap";
import DatePicker from "react-datepicker";
import Select from "react-select";
import { TextField } from '@material-ui/core';
import {RequestSendingService} from "../../Services/RequestSendingService";
import {URL} from "../../GlobalSettings/URL";
import {Link} from "react-router-dom";
import {DateFormatter} from "../../Services/DateFormatter";

const requestTypeOptions = [
    {value: 1, label: 'Paid holiday', sick: false, managerApproval: true},
    {value: 2, label: 'Admin (unpaid) planned', sick: false, managerApproval: true},
    {value: 3, label: 'Admin (unpaid) force majeure', sick: false, managerApproval: false},
    {value: 4, label: 'Study', sick: false, managerApproval: true},
    {value: 5, label: 'Social', sick: false, managerApproval: false},
    {value: 6, label: 'Sick with docs', sick: true, managerApproval: false},
    {value: 7, label: 'Sick without docs', sick: true, managerApproval: false}
];

const projectPartOptions = [
    {value: 1, label: 'No role'},
    {value: 2, label: 'Member'},
    {value: 3, label: 'Dedicated'},
    {value: 4, label: 'Representative'},
]

export class CreateRequest extends Component {
    static displayName = CreateRequest.name;

    constructor(props) {
        super(props);
        
        this.state = {
            managerRows: [{value: null, label: 'loading'}],
            request: null,
            sick: false,
            managerApproval: true,
            selectedProjectPart: projectPartOptions[0],
            textFieldProjectRole: "",
            textAreaComment: "",
            selectedRequestType: requestTypeOptions[0],
            selectedOptionDateFrom: new Date(),
            selectedOptionDateTo: new Date(),
            error: false,
            errorValue: "",
            loading: false,
            selectedManagers: null,
            ok: false
        }
        
        this._handleTextFiledProjectRoleChange = this._handleTextFiledProjectRoleChange.bind(this);
        this.sendRequest = this.sendRequest.bind(this);
        this.handleChangeTextAreaComment = this.handleChangeTextAreaComment.bind(this);
    }

    async componentDidMount(): Promise<void> {
        this.setState({
            loading: true,
        });
        await RequestSendingService.sendGetRequestAuthorized(URL.url + "Employee/GetManagers")
            .then(async response => {
                if (response.status === 200) {
                    const data: Array = await response.json().then(data => data);
                    console.log(data);
                    this.state.managerRows.pop();
                    data.forEach(manager => {
                        this.state.managerRows.push({
                            value: manager.id,
                            label: String(manager.firstName + " " + manager.secondName)
                        })
                    })
                    this.setState({
                        loading: false,
                        error: false
                    })
                }
            })
            .catch(error => {
                this.setState({
                    loading: false,
                    error: true
                })
                console.error(error);
            })
    }
    
    async sendRequest() {
        this.setState({
            loading: true,
        });
        await RequestSendingService.sendPostRequestAuthorized(URL.url + "Employee/CheckDateCollision", {
            dateTimeFrom: DateFormatter.dateToString(this.state.selectedOptionDateFrom),
            dateTimeTo: DateFormatter.dateToString(this.state.selectedOptionDateTo)
        })
            .then(async response => {
                if (response.status === 200) {
                    await this.afterDateCheck();
                }
                else {
                    const data = await response.json().then(data => data);
                    console.log(data);
                    this.setState({
                        loading: false,
                        error: true,
                        errorValue: (data.title === undefined) ? data : data.title,
                    })
                }
            })
            .catch(error => {
                this.setState({
                    loading: false,
                    error: true
                })
                console.error(error);
            })
    }
    
    async afterDateCheck() {
        this.setState({
            loading: true,
        });
        await RequestSendingService.sendPostRequestAuthorized(URL.url + "Employee/СreateRequest", {
            dateTimeFrom: DateFormatter.dateToString(this.state.selectedOptionDateFrom),
            dateTimeTo: DateFormatter.dateToString(this.state.selectedOptionDateTo),
            requestTypeId: this.state.selectedRequestType.value,
            reason: this.state.textAreaComment,
            projectRoleComment: this.state.textFieldProjectRole,
            projectRoleTypeId: this.state.selectedProjectPart.value,
            userSignature: this.state.selectedManagers?.map((manager, index) => ({
                nInQueue: index,
                userId: manager.value
            }))
        })
            .then(async res => {
                console.log(JSON.stringify({
                    dateTimeFrom: DateFormatter.dateToString(this.state.selectedOptionDateFrom),
                    dateTimeTo: DateFormatter.dateToString(this.state.selectedOptionDateTo),
                    requestTypeId: this.state.selectedRequestType.value,
                    reason: this.state.textAreaComment,
                    projectRoleComment: this.state.textFieldProjectRole,
                    projectRoleTypeId: this.state.selectedProjectPart.value,
                    userSignature: this.state.selectedManagers?.map((manager, index) => ({
                        nInQueue: index,
                        userId: manager.value
                    }))
                }))
                if (res.status === 200) {
                    this.setState({
                        loading: false,
                        error: false,
                        ok: true
                    })
                }
                else {
                    const d = await res.json().then(d => d);
                    console.log(d);
                    this.setState({
                        loading: false,
                        error: true,
                        errorValue: (d.title === undefined) ? d : d.title,
                    })
                }
            })
            .catch(e => {
                this.setState({
                    loading: false,
                    error: true,
                })
                console.error(e);
            })
    }

    render() {
        return (
            <div>
                <Container>
                    <Row>
                        <Col>
                            <center><p><strong>
                                New Request
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <center><p>
                                Request Type
                            </p></center>
                        </Col>
                        <Col>
                            <center><p>
                                Date From
                            </p></center>
                        </Col>
                        <Col>
                            <center><p>
                                Date To
                            </p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Select
                                value={this.state.selectedRequestType}
                                onChange={this._handleRequestTypeChange}
                                options={requestTypeOptions}
                                error={this.state.error}
                            />
                        </Col>
                        <Col>
                            <center>
                                <DatePicker
                                    showTimeSelect
                                    dateFormat="dd-MM-yyyy | HH:mm"
                                    selected={this.state.selectedOptionDateFrom}
                                    onChange={this.handleChangeDateFrom}
                                >
                                </DatePicker>
                            </center>
                        </Col>
                        <Col>
                            <center>
                                <DatePicker
                                    showTimeSelect
                                    dateFormat="dd-MM-yyyy | HH:mm"
                                    selected={this.state.selectedOptionDateTo}
                                    onChange={this.handleChangeDateTo}
                                >
                                </DatePicker>
                            </center>
                        </Col>
                    </Row>
                    <Row className="mt-3">
                        <Col>
                            <center><p>
                                My Comment
                            </p></center>
                        </Col>
                    </Row>
                    <Row className="mt-2">
                        <Col>
                            <textarea
                                onChange={this.handleChangeTextAreaComment}
                            />
                        </Col>
                    </Row>
                    <Row className="mt-3">
                        <Col>
                            <center><p>
                                Project Role
                            </p></center>
                        </Col>
                        <Col>
                            <center><p>
                                Project Participant
                            </p></center>
                        </Col>
                    </Row>
                    <Row className="mt-2">
                        <Col>
                            <TextField
                                value={this.state.textFieldProjectRole}
                                onChange={this._handleTextFiledProjectRoleChange}
                                required
                                label="Project Role"
                                autoFocus
                            />
                        </Col>
                        <Col>
                            <Select
                                value={this.state.selectedProjectPart}
                                onChange={this.handleChangeProjectPart}
                                options={projectPartOptions}
                                error={this.state.error}
                            />
                        </Col>
                    </Row>
                </Container>
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <center><p><strong>
                                Approvers
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Row>
                            <left><p><strong>
                                1. Accounting
                            </strong></p></left>
                        </Row>
                        {this.state.managerApproval &&
                        <Row>
                            <left><p><strong>
                                2. Others
                            </strong></p>
                            </left>
                            <Select
                                isMulti
                                options={this.state.managerRows}
                                className="basic-multi-select"
                                classNamePrefix="select"
                                value={this.state.selectedManagers}
                                onChange={this.handleChangeManagers}
                            />
                        </Row>
                        }
                    </Row>
                </Container>
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <center><p><strong>
                                {!this.state.error && this.state.ok && <font color="green"> Request has been sent! </font>}
                                {!this.state.error && !this.state.ok && "Create"}
                                {this.state.error && !this.state.ok && <font color="red"> {this.state.errorValue} </font>}
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Link to="/" style={{textDecoration: 'none'}}>
                                <Button
                                    outline
                                    block
                                    color="secondary">
                                    To My Requests Page
                                </Button>
                            </Link>
                        </Col>
                        <Col>
                            <Button
                                onClick={this.sendRequest}
                                block
                                outline
                                color="success">
                                Send Request
                            </Button>
                        </Col>
                    </Row>
                </Container>
            </div>
        );
    }

    _handleRequestTypeChange = selectedRequestType => {
        this.setState({selectedRequestType});
        this.setState({
            sick: selectedRequestType.sick,
            managerApproval: selectedRequestType.managerApproval
        })
    };

    handleChangeDateFrom = selectedOptionDateFrom => {
        this.setState({selectedOptionDateFrom});
    };

    handleChangeDateTo = selectedOptionDateTo => {
        this.setState({selectedOptionDateTo});
    };
    
    handleChangeTextAreaComment(e) {
        this.setState({
            textAreaComment: e.target.value
        })
    }

    handleChangeProjectPart = selectedProjectPart => {
        this.setState({selectedProjectPart: selectedProjectPart});
    };

    handleChangeManagers = selectedManagers => {
        this.setState({selectedManagers: selectedManagers});
    };

    _handleTextFiledProjectRoleChange(e) {
        this.setState({
            textFieldProjectRole: e.target.value
        });
    }
}