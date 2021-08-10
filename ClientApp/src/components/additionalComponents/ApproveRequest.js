import React, { Component } from 'react';
import {RequestSendingService} from "../../Services/RequestSendingService";
import {URL} from "../../GlobalSettings/URL";
import {getSearchParams} from "../../Helpers/UrlParametersParser";
import {Col, Container, Row} from "reactstrap";

export class ApproveRequest extends Component {
    static displayName = ApproveRequest.name;

    constructor(props) {
        super(props);

        this.state = {
            error: false,
            errorValue: "",
            loading: false,
            ok: false,
        }
    }
    
    async componentDidMount(): Promise<void> {
        this.setState({
            loading: true,
        });

        await RequestSendingService.sendGetRequestAuthorized(URL.url + "Manager/AcceptRequest?id=" + getSearchParams("id"))
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
                </Container>
            </div>
        );
    }
}