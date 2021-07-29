import React, {Component} from 'react';
import TextField from '@material-ui/core/TextField';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import {AuthService} from "../Services/AuthService";
import {Button} from "reactstrap";
import {RequestSendingService} from "../Services/RequestSendingService";
import {URL} from "../GlobalSettings/URL";

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
        
        console.log(URL + 'api/auth');

        await RequestSendingService.sendPostRequestUnauthorized(URL.url + 'api/auth', {
            login: this.state.textFieldLoginValue,
            password: this.state.textFieldPasswordValue
        }).then(async response => {
            if (response.status === 200) {
                const token = await response.json().then(token => token);
                localStorage.setItem("token", token);
                this.setState({
                    isLoading: false,
                })
                window.location.reload();
            } else {
                this.setState({
                    errorState: true
                });
                this.setState({
                    isLoading: false,
                })
            }
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
                <div>
                    <Typography component="h1" variant="h5" align="center">
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
                            outline
                            color="primary"
                            block
                            className="mt-2">
                            {this.state.isLoading ? "Signin In..." : "Sign In"}
                        </Button>
                        }
                        {AuthService.isLogged() &&
                        <Button
                            className="mt-2"
                            onClick={this._logOut}
                            outline
                            color="primary"
                            block
                            disabled={this.state.isLoading}>
                            {this.state.isLoading ? "Logging out..." : "Log out"}
                        </Button>
                        }
                    </form>
                </div>
            </Container>
        );
    }
}