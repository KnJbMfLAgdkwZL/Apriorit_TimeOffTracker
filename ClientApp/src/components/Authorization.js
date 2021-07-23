﻿import React, {Component} from 'react';
import Avatar from '@material-ui/core/Avatar';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import TextField from '@material-ui/core/TextField';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import {AuthService} from "../Services/AuthService";
import Cookies from 'js-cookie';
import '../custom.css'

const URL = "http://localhost:5000/"

export class Authorization extends Component {

    static displayName = Authorization.name;

    constructor(props) {
        super(props);

        this.state = {
            textFieldLoginValue: "",
            textFieldPasswordValue: "",
            isLoading: false,
            errorState: false
        };

        this._handleTextFiledLoginChange = this._handleTextFiledLoginChange.bind(this);
        this._handleTextFiledPasswordChange = this._handleTextFiledPasswordChange.bind(this);
        this._sendPostRequest = this._sendPostRequest.bind(this);
        this._logOut = this._logOut.bind(this);
    }

    async _sendPostRequest() {
        this.setState({
            isLoading: true,
        })
        const requestOptions = {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({login: this.state.textFieldLoginValue, password: this.state.textFieldPasswordValue}),
            redirect: 'manual'
        };
        await fetch(URL + 'api/auth', requestOptions)
            .then(async response => {
                if (response.status === 200) {
                    const token = await response.json().then(token => token);
                    Cookies.set("token", token);
                    this.setState({
                        isLoading: false,
                    })
                    window.location.reload();
                } else {
                    this.setState({
                        errorState: true
                    });
                }
            })
            .catch(error => console.error(error));
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

    _logOut() {
        this.setState({
            isLoading: true,
        })
        AuthService.logOut();
        this.setState({
            isLoading: false,
        })
        window.location.reload();
    }

    render() {
        return (
            <Container component="main" maxWidth="xs">
                <CssBaseline/>
                <div>
                    <Avatar>
                        <LockOutlinedIcon/>
                    </Avatar>
                    <Typography component="h1" variant="h5">
                        Sign in {this.state.errorState && "again please..."}
                    </Typography>
                    <form noValidate>
                        {!AuthService.isLogged() &&
                        <TextField
                            error={this.state.errorState}
                            value={this.state.textFieldLoginValue}
                            onChange={this._handleTextFiledLoginChange}
                            margin="normal"
                            required
                            fullWidth
                            id="login"
                            label="Login"
                            name="login"
                            autoComplete="login"
                            autoFocus
                        />
                        }
                        {!AuthService.isLogged() &&
                        <TextField
                            error={this.state.errorState}
                            value={this.state.textFieldPasswordValue}
                            onChange={this._handleTextFiledPasswordChange}
                            margin="normal"
                            required
                            fullWidth
                            name="password"
                            label="Password"
                            type="password"
                            id="password"
                        />
                        }
                        {!AuthService.isLogged() &&
                        <Button
                            onClick={this._sendPostRequest}
                            fullWidth
                            variant="contained"
                            color="primary"
                            className="mt-1">
                            {this.state.isLoading ? "Signin In..." : "Sign In"}
                        </Button>
                        }
                        {AuthService.isLogged() &&
                        <Button
                            onClick={this._logOut}
                            fullWidth
                            variant="outlined"
                            color="primary"
                            className="mt-2">
                            {this.state.isLoading ? "Logging out..." : "Log out"}
                        </Button>
                        }
                    </form>
                </div>
            </Container>
        );
    }
}