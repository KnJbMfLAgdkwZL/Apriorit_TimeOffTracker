import React, {Component} from 'react';
import {Container, Row, Col, Button} from 'reactstrap';
import Select from "react-select";
import {RequestSendingService} from "../../Services/RequestSendingService";
import {URL} from "../../GlobalSettings/URL";

const roleOptions = [
    {value: 3, label: 'Employee'},
    {value: 4, label: 'Manager'}
];

export class EditRole extends Component {
    static displayName = EditRole.name;

    constructor(props) {
        super(props);

        this.state = {
            selectedRoleOption: null,
            error: false,
            loading: false
        };

        this._editRole = this._editRole.bind(this);
    }

    render() {
        return (
            <Container className="mt-3">
                <Row>
                    <Col>
                        <center><p><strong>
                            {this.state.error ? <font color="red"> Something went wrong!</font> : "Edit Stage"}
                        </strong></p></center>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <center><p>User Info</p></center>
                    </Col>
                    <Col/>
                    <Col/>
                    <Col>
                        <center><p>Chose User Role</p></center>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <strong>name:</strong> {this.props.user.name}<br/>
                        <strong>email:</strong> {this.props.user.email}<br/>
                        <strong>login:</strong> {this.props.user.login}<br/>
                        <strong>role:</strong> {this.props.user.role}
                    </Col>
                    <Col>
                        <center>
                            <Button
                                onClick={this._editRole}
                                outline
                                block
                                color="info">
                                {this.state.loading ? "Loading..." : "Apply"}
                            </Button>
                        </center>
                    </Col>
                    <Col>
                        <center>
                            <Button
                                onClick={this.props.closeHandler}
                                outline
                                block
                                color="danger">
                                {this.state.loading ? "Loading..." : "Cancel"}
                            </Button>
                        </center>
                    </Col>
                    <Col>
                        <Select
                            value={this.state.selectedRoleOption}
                            onChange={this._handleRoleChange}
                            options={roleOptions}
                        />
                    </Col>
                </Row>
            </Container>
        );
    }

    _handleRoleChange = selectedRoleOption => {
        this.setState({selectedRoleOption});
    };

    async _editRole() {
        this.setState({
            loading: true
        })
        await RequestSendingService.sendPatchRequestAuthorized(URL.url + "Admin/ModifyUserRole", {
            id: parseInt(this.props.user.id),
            roleId: this.state.selectedRoleOption.value
        })
            .then(response => {
                if (response.status === 200) {
                    this.props.closeHandler();
                    this.setState({
                        error: false,
                        loading: false
                    })
                } else {
                    this.setState({
                        error: true,
                        loading: false
                    })
                }
            })
            .catch(error => {
                console.error(error);
                this.setState({
                    error: true,
                    loading: false
                })
            }).then(r => {window.location.reload()})
    }
}