﻿import React, {Component} from 'react';
import {RequestSendingService} from "../../Services/RequestSendingService";
import {Button, Col, Container, Row} from "reactstrap";
import Select from "react-select";
import TextField from "@material-ui/core/TextField";
import {Link} from "react-router-dom";

const URL = "http://localhost:5000/";

const roleOptions = [
    {value: '4', label: 'Manager'},
    {value: '3', label: 'Employee'}
];

export class CreateUser extends Component {
    static displayName = CreateUser.name;

    constructor(props) {
        super(props);

        this.state = {
            textFieldEmailValue: "",
            textFieldLoginValue: "",
            textFieldPasswordValue: "",
            textFieldFirstNameValue: "",
            textFieldSecondNameValue: "",
            selectedRoleOption: roleOptions[1],
            add: false,
            error: false,
            loading: false
        };

        this._handleTextFiledEmailChange = this._handleTextFiledEmailChange.bind(this);
        this._handleTextFiledLoginChange = this._handleTextFiledLoginChange.bind(this);
        this._handleTextFiledPasswordChange = this._handleTextFiledPasswordChange.bind(this);
        this._handleTextFiledFirstNameChange = this._handleTextFiledFirstNameChange.bind(this);
        this._handleTextFiledSecondNameChange = this._handleTextFiledSecondNameChange.bind(this);
        this._createUser = this._createUser.bind(this);
    }

    render() {
        return (
            <div>
                <Container>
                    <Row>
                        <Col>
                            <center><p><strong>
                                {!this.state.add && !this.state.error && "User creation page"}
                                {this.state.add && <font color="green"> User was successfully created!</font>}
                                {this.state.error && <font color="red"> Something went wrong!</font>}
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <TextField
                                value={this.state.textFieldEmailValue}
                                onChange={this._handleTextFiledEmailChange}
                                margin="normal"
                                required
                                fullWidth
                                id="Email"
                                name="Email"
                                label="Email"
                                autoFocus
                                error={this.state.error}
                            />
                        </Col>
                        <Col>
                            <TextField
                                value={this.state.textFieldLoginValue}
                                onChange={this._handleTextFiledLoginChange}
                                margin="normal"
                                required
                                fullWidth
                                label="Login"
                                autoFocus
                                error={this.state.error}
                            />
                        </Col>
                        <Col>
                            <TextField
                                value={this.state.textFieldPasswordValue}
                                onChange={this._handleTextFiledPasswordChange}
                                margin="normal"
                                required
                                fullWidth
                                label="Password"
                                autoFocus
                                error={this.state.error}
                            />
                        </Col>
                    </Row>
                </Container>
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <TextField
                                value={this.state.textFieldFirstNameValue}
                                onChange={this._handleTextFiledFirstNameChange}
                                margin="normal"
                                required
                                fullWidth
                                label="First Name"
                                autoFocus
                                error={this.state.error}
                            />
                        </Col>
                        <Col>
                            <TextField
                                value={this.state.textFieldSecondNameValue}
                                onChange={this._handleTextFiledSecondNameChange}
                                margin="normal"
                                required
                                fullWidth
                                label="Second Name"
                                autoFocus
                                error={this.state.error}
                            />
                        </Col>
                        <Col>
                            <Select
                                className="mt-3"
                                value={this.state.selectedRoleOption}
                                onChange={this._handleRoleChange}
                                options={roleOptions}
                            />
                        </Col>
                    </Row>
                </Container>
                <Container className="mt-5">
                    <Row>
                        <Col>
                            <center><p><strong>
                                Final Stage
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col/>
                        <Col>
                            <center>
                                <Button
                                    onClick={this._createUser}
                                    disabled={this.state.loading}
                                    outline
                                    block
                                    color="success">
                                    {!this.state.loading ? "Create" : "Loading..."}
                                </Button>
                            </center>
                        </Col>
                        <Col/>
                    </Row>
                </Container>
                <Container className="mt-3">
                    <Row>
                        <Col/>
                        <Col>
                            <center>
                                <Link to="/" style={{textDecoration: 'none'}}>
                                    <Button
                                        outline
                                        block
                                        color="info">
                                        To Users Page
                                    </Button>
                                </Link>
                            </center>
                        </Col>
                        <Col/>
                    </Row>
                </Container>
            </div>
        );
    }

    async _createUser() {
        this.setState({
            loading: true,
        })
        await RequestSendingService.sendPostRequestAuthorized(URL + "Admin/CreateUser", {
            "email": this.state.textFieldEmailValue,
            "login": this.state.textFieldLoginValue,
            "firstName": this.state.textFieldFirstNameValue,
            "secondName": this.state.textFieldSecondNameValue,
            "password": this.state.textFieldPasswordValue,
            "roleId": parseInt(this.state.selectedRoleOption.value)
        })
            .then(response => {
                if (response.status === 200) {
                    this.setState({
                        add: true,
                        error: false,
                        loading: false
                    })
                    this._cleanSendBodyStates();
                } else {
                    this.setState({
                        add: false,
                        error: true,
                        loading: false
                    })
                }
            })
            .catch(error => {
                console.error(error);
                this.setState({
                    error: true,
                    loading: false,
                })
            })
    }

    _handleTextFiledEmailChange(e) {
        this.setState({
            textFieldEmailValue: e.target.value
        });
    }

    _handleTextFiledLoginChange(e) {
        this.setState({
            textFieldLoginValue: e.target.value
        });
    }

    _handleTextFiledPasswordChange(e) {
        this.setState({
            textFieldPasswordValue: e.target.value
        });
    }

    _handleTextFiledFirstNameChange(e) {
        this.setState({
            textFieldFirstNameValue: e.target.value
        });
    }

    _handleTextFiledSecondNameChange(e) {
        this.setState({
            textFieldSecondNameValue: e.target.value
        });
    }

    _handleRoleChange = selectedRoleOption => {
        this.setState({selectedRoleOption});
    };

    _cleanSendBodyStates() {
        this.setState({
            textFieldEmailValue: "",
            textFieldLoginValue: "",
            textFieldPasswordValue: "",
            textFieldFirstNameValue: "",
            textFieldSecondNameValue: "",
            selectedRoleOption: roleOptions[1],
        });
    }
}