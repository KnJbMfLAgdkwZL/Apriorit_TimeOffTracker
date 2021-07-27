import React, {Component} from 'react';
import {Container, Row, Col, Button} from 'reactstrap';
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';
import DataGrid from "react-data-grid"
import {RequestSendingService} from "../Services/RequestSendingService";

const URL = "http://localhost:5000/";

const columns = [
    {key: 'name', name: 'name'},
    {key: 'email', name: 'email'},
    {key: 'login', name: 'login'},
    {key: 'role', name: 'role'}
];

const roleOptions = [
    {value: null, label: 'Any'},
    {value: 'Admin', label: 'Admin'},
    {value: 'Accounting', label: 'Accounting'},
    {value: 'Manager', label: 'Manager'},
    {value: 'Employee', label: 'Employee'}
];

export class Users extends Component {
    static displayName = Users.name;

    constructor(props) {
        super(props);

        this.state = {
            nameOptions: [
                {value: null, label: 'Any'},
            ],
            emailOptions: [
                {value: null, label: 'Any'},
            ],
            rows: [
                {name: 0, email: 'Example', login: "@123", role: "admin?"},
            ],
            selectedNameOption: null,
            selectedRoleOption: null,
            selectedEmailOption: null
        };
    }

    _handleNameChange = selectedNameOption => {
        this.setState({selectedNameOption});
    };

    _handleRoleChange = selectedRoleOption => {
        this.setState({selectedRoleOption});
    };

    _handleEmailChange = selectedEmailOption => {
        this.setState({selectedEmailOption});
    };

    async componentDidMount(): Promise<void> {
        this.setState({
            selectedNameOption: this.state.nameOptions[0],
            selectedEmailOption: this.state.emailOptions[0],
            selectedRoleOption: roleOptions[0]
        });
        
        await RequestSendingService.sendPostRequestAuthorized(URL + "Admin/GetUsers?page=1&pageSize=10000", {})
            .then(async response => {
                if (response.status === 200) {
                    try {
                        const data = await response.json().then(data => data);
                        this.state.rows.pop();
                        data.users.forEach(user => {
                            this.state.rows.push({
                                name: String(user.firstName + " " + user.secondName),
                                role: user.roleId,
                                email: user.email,
                                login: user.login
                            });
                            this.state.nameOptions.push({
                                value: String(user.firstName + " " + user.secondName),
                                label: String(user.firstName + " " + user.secondName)
                            });
                            this.state.emailOptions.push({
                                value: user.email,
                                label: user.email
                            });
                        });
                        this.setState({});
                    }
                    catch (error) {
                        console.error(error);
                    }
                }
            })
            .catch(error => {
                console.error(error);
            })
    }

    render() {
        return (
            <div>
                <Container>
                    <Row>
                        <Col>
                            <center><p><strong>
                                Filter Stage
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <center><p>Name</p></center>
                        </Col>
                        <Col>
                            <center><p>Email</p></center>
                        </Col>
                        <Col>
                            <center><p>Role</p></center>
                        </Col>
                        <Col/>
                    </Row>
                    <Row>
                        <Col>
                            <Select
                                value={this.state.selectedNameOption}
                                onChange={this._handleNameChange}
                                options={this.state.nameOptions}
                            />
                        </Col>
                        <Col>
                            <Select
                                value={this.state.selectedEmailOption}
                                onChange={this._handleEmailChange}
                                options={this.state.emailOptions}
                            />
                        </Col>
                        <Col>
                            <Select
                                value={this.state.selectedRoleOption}
                                onChange={this._handleRoleChange}
                                options={roleOptions}
                            />
                        </Col>
                        <Col>
                            <Button
                                outline
                                block
                                color="info">
                                Apply Filter
                            </Button>
                        </Col>
                    </Row>
                </Container>
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <center><p><strong>
                                Stage Of Change
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>

                    </Row>
                </Container>
                <DataGrid
                    columns={columns}
                    rows={this.state.rows}
                />
            </div>
        );
    }
}