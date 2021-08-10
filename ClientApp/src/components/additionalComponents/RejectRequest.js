import React, { Component } from 'react';
import TextField from "@material-ui/core/TextField";
import {RequestSendingService} from "../../Services/RequestSendingService";
import {URL} from "../../GlobalSettings/URL";
import {getSearchParams} from "../../Helpers/UrlParametersParser";
import {Button, Col, Container, Row} from "reactstrap";

export class RejectRequest extends Component {
    static displayName = RejectRequest.name;

    constructor(props) {
        super(props);

        this.state = {
            textFieldReasonValue: "",
            error: false,
            errorValue: "",
            loading: false,
            ok: false,
        }

        this.sendRejectRequest = this.sendRejectRequest.bind(this);
    }

    render() {
        return (
            <div>
                <Container>
                    <Row>
                        <Col>
                            <center><p><strong>
                                Approve Page
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <center><p><strong>
                                Result:
                                {!this.state.error && this.state.ok && <font color="green"> Request has been sent! </font>}
                                {this.state.error && <font color="red"> {this.state.errorValue} </font>}
                            </strong></p></center>
                        </Col>
                    </Row>                    
                    <Row>
                        <Col>
                            <TextField
                                value={this.state.textFieldReasonValue}
                                onChange={this._handleTextFiledReasonChange}
                                margin="normal"
                                required
                                fullWidth
                                id="Reason"
                                name="Reason"
                                label="Reason"
                                helperText="Specify the reason of rejection"
                                error={this.state.error}
                            />
                        </Col>
                        <Col>
                            <Button outline
                                    onClick={this.sendRejectRequest}
                                    block
                                    color="danger">
                                To Reject Page
                            </Button>
                        </Col>
                    </Row>
                </Container>
            </div>
        );
    }

    _handleTextFiledReasonChange(e) {
        this.setState({
            textFieldReasonValue: e.target.value
        });
    }

    async sendRejectRequest() {
        this.setState({
            loading: true,
        });

        await RequestSendingService.sendPostRequestAuthorized(URL.url + "Manager/RejectRequest?id=" + getSearchParams("id"), {
            reason: this.state.textFieldReasonValue
        })
            .then(async res => {
                if (res.status === 200) {
                    this.setState({
                        loading: false,
                        error: false,
                        ok: true
                    })
                }
                else if (res.status === 500) {
                    this.setState({
                        loading: false,
                        error: true,
                        errorValue: "Server error...",
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
}