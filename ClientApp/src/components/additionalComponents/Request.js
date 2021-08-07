import React, {Component} from 'react';
import {RequestSendingService} from "../../Services/RequestSendingService";
import {URL} from "../../GlobalSettings/URL";
import {DateFormatter} from "../../Services/DateFormatter";
import {Button, Col, Container, Row} from "reactstrap";
import Select from "react-select";
import DatePicker from "react-datepicker";
import {TextField} from "@material-ui/core";
import {Link, Route, Switch} from "react-router-dom";
import {Layout} from "../Layout";
import {MyRequests} from "../MyRequests";
import {AuthService} from "../../Services/AuthService";
import {Redirect} from "react-router";
import {Authorization} from "../Authorization";
import {CreateRequest} from "./CreateRequest";
import equal from 'fast-deep-equal';
import {Markup} from 'interweave';

const styles = {
    multiValue: (base, state) => {
        return state.data.isFixed ? {...base, backgroundColor: "gray"} : base;
    },
    multiValueLabel: (base, state) => {
        return state.data.isFixed
            ? {...base, color: "white", paddingRight: 5}
            : base;
    },
    multiValueRemove: (base, state) => {
        return state.data.isFixed ? {...base, display: "none"} : base;
    }
};

const requestTypeOptions = [
    {value: 1, label: 'Paid holiday', sick: false, managerApproval: true},
    {value: 2, label: 'Admin (unpaid) planned', sick: false, managerApproval: true},
    {value: 3, label: 'Admin (unpaid) force majeure', sick: false, managerApproval: false},
    {value: 4, label: 'Study', sick: false, managerApproval: true},
    {value: 5, label: 'Social', sick: false, managerApproval: false},
    {value: 6, label: 'Sick with docs', sick: true, managerApproval: false},
    {value: 7, label: 'Sick without docs', sick: true, managerApproval: false}
];

const sickDayBusynessOptions = [
    {value: 'Full day', label: 'Full day'},
    {value: 'Part of the day', label: 'Part of the day'},
]

const projectPartOptions = [
    {value: 1, label: 'No role'},
    {value: 2, label: 'Member'},
    {value: 3, label: 'Dedicated'},
    {value: 4, label: 'Representative'},
]

export class Request extends Component {
    static displayName = Request.name;

    constructor(props) {
        super(props);

        this.state = {
            managerRows: [{value: null, label: 'loading'}],
            request: null,
            sick: false,
            managerApproval: true,
            sickDayBusyness: "",
            selectedProjectPart: null,
            textFieldProjectRole: "",
            textAreaComment: null,
            selectedRequestType: null,
            selectedOptionDateFrom: null,
            selectedOptionDateTo: null,
            error: false,
            errorValue: "",
            loading: false,
            selectedManagers: [],
            ok: false,
            edit: false,
            requestStateId: "",
            accountingApprove: null,

            renderedManagers: "",
        }

        this._handleTextFiledProjectRoleChange = this._handleTextFiledProjectRoleChange.bind(this);
        this.sendRequest = this.sendRequest.bind(this);
        this.handleChangeTextAreaComment = this.handleChangeTextAreaComment.bind(this);
        this._renderManagerList = this._renderManagerList.bind(this);
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

        await RequestSendingService.sendGetRequestAuthorized(URL.url + "Employee/GetRequestDetails" + window.location.search)
            .then(async response => {
                if (response.status === 200) {
                    const data: Array = await response.json().then(data => data);
                    console.log(data);
                    this.setState({
                        loading: false,
                        error: false,
                        selectedOptionDateFrom: Date.parse(data.dateTimeFrom),
                        selectedOptionDateTo: Date.parse(data.dateTimeTo),
                        selectedRequestType: requestTypeOptions.find(type => type.value === data.requestTypeId),
                        textAreaComment: data.reason,
                        textFieldProjectRole: data.projectRoleComment,
                        selectedProjectPart: projectPartOptions.find(option => option.value === data.projectRoleTypeId),
                        requestStateId: data.stateDetailId,
                        selectedManagers: data.userSignature.slice(1).filter(us => us.deleted === false).forEach(us => {
                            this.state.selectedManagers.push({
                                value: us.user.id, label: String(us.user.firstName + " " + us.user.secondName),
                                isFixed: us.approved === true && us.deleted === false
                            })
                        }),
                        accountingApprove: data.userSignature[0].approved,

                        renderedManagers: this._renderManagerList(data.userSignature.slice(1).filter(us => us.deleted === false)),
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
                } else {
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
                                isDisabled={!this.state.edit}
                            />
                        </Col>
                        <Col>
                            <center>
                                <DatePicker
                                    showTimeSelect
                                    dateFormat="dd-MM-yyyy"
                                    selected={this.state.selectedOptionDateFrom}
                                    onChange={this.handleChangeDateFrom}
                                    disabled={!this.state.edit}
                                >
                                </DatePicker>
                            </center>
                        </Col>
                        <Col>
                            <center>
                                <DatePicker
                                    showTimeSelect
                                    dateFormat="dd-MM-yyyy"
                                    selected={this.state.selectedOptionDateTo}
                                    onChange={this.handleChangeDateTo}
                                    disabled={!this.state.edit}
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
                        {this.state.sick &&
                        <Col>
                            <center><p>
                                Use
                            </p></center>
                        </Col>
                        }
                    </Row>
                    <Row className="mt-2">
                        <Col>
                            <textarea
                                placeholder={this.state.textAreaComment}
                                onChange={this.handleChangeTextAreaComment}
                                disabled={!this.state.edit}
                            />
                        </Col>
                        {this.state.sick &&
                        <Col>
                            <Select
                                value={this.state.sickDayBusyness}
                                onChange={this.handleChangeSickDayBusyness}
                                options={sickDayBusynessOptions}
                                error={this.state.error}
                                isDisabled={!this.state.edit}
                            />
                        </Col>
                        }
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
                                error={this.state.error}
                                disabled={!this.state.edit}
                            />
                        </Col>
                        <Col>
                            <Select
                                value={this.state.selectedProjectPart}
                                onChange={this.handleChangeProjectPart}
                                options={projectPartOptions}
                                error={this.state.error}
                                isDisabled={!this.state.edit}
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
                        {this.state.managerApproval &&
                        <Row>
                            <left><p>
                                1. Accounting
                                - <strong>{this.state.accountingApprove ? "approved" : "not approved"}</strong><br/>
                                <Markup content={this.state.renderedManagers}/>
                            </p><p>
                                Click on select to see the approvers. Dark grey - already approved.
                            </p>
                            </left>
                            <Select
                                isMulti
                                options={this.state.managerRows}
                                className="basic-multi-select"
                                classNamePrefix="select"
                                isClearable={false}
                                value={this.state.selectedManagers}
                                onChange={this.handleChangeManagers}
                                styles={styles}
                                isDisabled={!this.state.edit}
                                closeMenuOnSelect={false}
                            />
                        </Row>
                        }
                    </Row>
                </Container>
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <center><p><strong>
                                {!this.state.error && this.state.ok &&
                                <font color="green"> Request has been sent! </font>}
                                {!this.state.error && !this.state.ok && "Create"}
                                {this.state.error && !this.state.ok &&
                                <font color="red"> {this.state.errorValue} </font>}
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
                                    Back
                                </Button>
                            </Link>
                        </Col>
                        {!this.state.edit &&
                        <Col>
                            <Button
                                onClick={() => this.setState({edit: !this.state.edit})}
                                block
                                outline
                                color="info">
                                Edit
                            </Button>
                        </Col>
                        }
                        {!this.state.edit &&
                        <Col>
                            <Button
                                // onClick={}
                                block
                                outline
                                color="success">
                                Duplicate
                            </Button>
                        </Col>
                        }
                        {!this.state.edit &&
                        <Col>
                            <Button
                                // onClick={this.sendRequest}
                                block
                                outline
                                color="danger">
                                Decline
                            </Button>
                        </Col>
                        }
                        {this.state.edit &&
                        <Col>
                            <Button
                                // onClick={this.sendRequest}
                                block
                                outline
                                color="success">
                                Save
                            </Button>
                        </Col>
                        }
                        {this.state.edit &&
                        <Col>
                            <Button
                                onClick={() => this.setState({edit: !this.state.edit})}
                                block
                                outline
                                color="danger">
                                Cancel
                            </Button>
                        </Col>
                        }
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

    handleChangeSickDayBusyness = sickDayBusyness => {
        this.setState({sickDayBusyness});
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

    _renderManagerList(managers) {
        let result = "";
        if (managers !== undefined) {
            managers.forEach((m, index) => (result += (index + 2) + ". " + m.user.firstName + " " + m.user.secondName + 
                " - <strong>" + (!m.approved && "not") + " approved</strong></br>"));
        }
        return result;
    }
}