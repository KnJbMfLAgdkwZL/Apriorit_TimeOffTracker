import React, {Component} from 'react';
import {Container, Row, Col, Button} from 'reactstrap';
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';
import DataGrid from "react-data-grid"
import {RequestSendingService} from "../Services/RequestSendingService";
import {Link} from "react-router-dom";
import {UserRoleEnum} from "../Enums/UserRoleEnum";
import {EditRole} from "./additionalComponents/EditRole";
import {URL} from "../GlobalSettings/URL";

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
                {id: "", name: "loading...", email: 'loading...', login: "loading...", role: "loading...", visible: true},
            ],
            selectedNameOption: null,
            selectedRoleOption: null,
            selectedEmailOption: null,
            edit: false
        };

        this._filterBySelects = this._filterBySelects.bind(this);
        this._openEditUserPart = this._openEditUserPart.bind(this);
        this._closeEditUserPart = this._closeEditUserPart.bind(this);
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
                                onClick={this._filterBySelects}
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
                    rows={this.state.rows.filter(row => row.visible)}
                />
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <center><p><strong>
                                Create Stage
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col/>
                        <Col>
                            <center>
                                <Link to="/createUser" style={{textDecoration: 'none'}}>
                                    <Button
                                        outline
                                        block
                                        color="success">
                                        New User
                                    </Button>
                                </Link>
                            </center>
                        </Col>
                        <Col>
                            <center>
                                <Button
                                    onClick={this._openEditUserPart}
                                    outline
                                    block
                                    color="success">
                                    Edit User By Chosen Email
                                </Button>
                            </center>
                        </Col>
                        <Col/>
                    </Row>
                </Container>
                {this.state.edit &&
                    <EditRole
                        closeHandler={this._closeEditUserPart}
                        user={this.state.rows.find((element, index, arr) => {
                            return element.visible;
                        })}
                    />
                }
            </div>
        );
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

        await RequestSendingService.sendPostRequestAuthorized(URL.url + "Admin/GetUsers?page=1&pageSize=10000", {})
            .then(async response => {
                if (response.status === 200) {
                    try {
                        const data = await response.json().then(data => data);
                        this.state.rows.pop();
                        data.users.forEach(user => {
                            this.state.rows.push({
                                id: user.id,
                                name: String(user.firstName + " " + user.secondName),
                                role: UserRoleEnum[user.roleId],
                                email: user.email,
                                login: user.login,
                                visible: true
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
                    } catch (error) {
                        console.error(error);
                    }
                }
            })
            .catch(error => {
                console.error(error);
            })
    }

    _filterBySelects() {
        this.state.rows.forEach(row => {
            let result = true;

            if ((this.state.selectedNameOption.value !== null) && row.name !== this.state.selectedNameOption.value) {
                result = result && false;
            } else {
                result = result && true;
            }

            if ((this.state.selectedEmailOption.value !== null) && row.email !== this.state.selectedEmailOption.value) {
                result = result && false;
            } else {
                result = result && true;
            }

            if ((this.state.selectedRoleOption.value !== null) && row.role !== this.state.selectedRoleOption.value) {
                result = result && false;
            } else {
                result = result && true;
            }

            row.visible = result;
        })
        this.setState({})
    }

    _openEditUserPart() {
        this.setState({
            edit: true
        })
    }

    _closeEditUserPart() {
        this.setState({
            edit: false
        })
    }
}