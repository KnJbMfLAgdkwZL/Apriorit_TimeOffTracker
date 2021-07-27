import React, {Component} from 'react';
import {RequestSendingService} from "../../Services/RequestSendingService";
import {Button, Col, Container, Row} from "reactstrap";
import Select from "react-select";
import TextField from "@material-ui/core/TextField";
import {Link} from "react-router-dom";

const URL = "http://localhost:5000/";

const roleOptions = [
    {value: '3', label: 'Manager'},
    {value: '4', label: 'Employee'}
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
            selectedRoleOption: null,
        };
        
        this._handleTextFiledEmailChange = this._handleTextFiledEmailChange.bind(this);
        this._handleTextFiledLoginChange = this._handleTextFiledLoginChange.bind(this);
        this._handleTextFiledPasswordChange = this._handleTextFiledPasswordChange.bind(this);
        this._handleTextFiledFirstNameChange = this._handleTextFiledFirstNameChange.bind(this);
        this._handleTextFiledSecondNameChange = this._handleTextFiledSecondNameChange.bind(this);
    }

    render() {
        return (
            <div>
                <Container>
                    <Row>
                        <Col>
                            <center><p><strong>
                                User creation page
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
                                label="Email"
                                autoFocus
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
                                    outline
                                    block
                                    color="success">
                                    Create
                                </Button>
                            </center>
                        </Col>
                        <Col/>
                    </Row>
                </Container>
            </div>
        );
    }
    
    _createUser() {
        // RequestSendingService.sendPostRequestAuthorized(URL, )
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
}