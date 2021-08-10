import React, {Component} from 'react';
import {Button, Col, Container, Row} from 'reactstrap';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';
import {RequestSendingService} from "../Services/RequestSendingService";
import {URL} from "../GlobalSettings/URL";
import DataGrid from "react-data-grid";
import {Link} from "react-router-dom";
import {StateDetailts} from "../Enums/StateDetailts";
import {RequestTypes} from "../Enums/RequestTypes";

const columns = [
    {key: 'id', name: 'id', width: '5%'},
    {key: 'state', name: 'state', width: '9%'},
    {key: 'type', name: 'type', width: '15%'},
    {key: 'dateFrom', name: 'dateFrom', width: '10%'},
    {key: 'dateTo', name: 'dateTo', width: '10%'},
    {key: 'comment', name: 'my comment', width: '18%'},
    {key: 'details', name: 'state details', width: '30%'},
];

const stateOptions = [
    {value: null, label: 'Any'},
    {value: 1, label: StateDetailts["1"]},
    {value: 2, label: StateDetailts["2"]},
    {value: 3, label: StateDetailts["3"]},
    {value: 4, label: StateDetailts["4"]},
    {value: 5, label: StateDetailts["5"]},
    {value: 6, label: StateDetailts["6"]},
    {value: 7, label: StateDetailts["7"]},
]

const typeOptions = [
    {value: null, label: 'Any'},
    {value: 1, label: RequestTypes["1"]},
    {value: 2, label: RequestTypes["2"]},
    {value: 3, label: RequestTypes["3"]},
    {value: 4, label: RequestTypes["4"]},
    {value: 5, label: RequestTypes["5"]},
    {value: 6, label: RequestTypes["6"]},
    {value: 7, label: RequestTypes["7"]}
]

export class OthersRequests extends Component {
    static displayName = OthersRequests.name;

    constructor(props) {
        super(props);

        this.state = {
            selectedOptionDateFrom: new Date().setMonth(new Date().getMonth() - 2),
            selectedOptionDateTo: new Date().setMonth(new Date().getMonth() + 2),
            selectedOptionState: null,
            selectedOptionType: null,
            selectedOptionId: null,
            rows: [
                {
                    id: "loading...",
                    state: "loading...",
                    type: 'loading...',
                    dateFrom: "loading...",
                    dateTo: "loading...",
                    comment: "loading...",
                    details: "loading...",
                    visible: true
                },
            ],
            loading: false,
            error: false,
            chosenRequest: null,
            idOptions: [{value: null, label: 'Any'}],
        };
        
        this._filter = this._filter.bind(this);
    }

    render() {
        return (
            <div>
                <Container>
                    <Row>
                        <Col>
                            <center><p><strong>
                                New Or View
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Link to={"/approveRequest?id=" + this.state.chosenRequest?.id} style={{textDecoration: 'none'}}>
                                <Button
                                    outline
                                    block
                                    color="success">
                                    To Approve Page
                                </Button>
                            </Link>
                        </Col>
                        <Col>
                            <center><p><strong>Chosen request (id): </strong>{this.state.chosenRequest?.id}</p></center>
                        </Col>
                        <Col>
                            <Link to={"/rejectRequest?id=" + this.state.chosenRequest?.id} style={{textDecoration: 'none'}}>
                                <Button className="mt-1"
                                    outline
                                    block
                                    color="danger">
                                    To Reject Page
                                </Button>
                            </Link>
                        </Col>
                    </Row>
                </Container>
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <center><p><strong>
                                Others' Requests Which You Can Approve Or Reject
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <center><p>Dates From / To</p></center>
                        </Col>
                        <Col>
                            <center><p>State / Type / Id</p></center>
                        </Col>
                        <Col/>
                    </Row>
                    <Row>
                        <Col>
                            <center>
                                <DatePicker
                                    dateFormat="dd-MM-yyyy"
                                    selected={this.state.selectedOptionDateFrom}
                                    onChange={this.handleChangeDateFrom}
                                >
                                </DatePicker>
                                <DatePicker
                                    dateFormat="dd-MM-yyyy"
                                    selected={this.state.selectedOptionDateTo}
                                    onChange={this.handleChangeDateTo}
                                    className="mt-2"
                                >
                                </DatePicker>
                            </center>
                        </Col>
                        <Col>
                            <Select
                                value={this.state.selectedOptionState}
                                onChange={this.handleChangeState}
                                options={stateOptions}
                            />
                            <Select
                                value={this.state.selectedOptionType}
                                onChange={this.handleChangeType}
                                options={typeOptions}
                                className="mt-2"
                            />
                            <Select
                                value={this.state.selectedOptionId}
                                onChange={this.handleChangeId}
                                options={this.state.idOptions}
                                className="mt-2"
                            />
                        </Col>
                        <Col>
                            <Button
                                onClick={this._filter}
                                block
                                outline
                                color="info">
                                Apply Filter
                            </Button>
                        </Col>
                    </Row>
                </Container>
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <DataGrid
                                columns={columns}
                                rows={this.state.rows.filter(row => row.visible)}
                            />
                        </Col>
                    </Row>
                </Container>
            </div>
        );
    }

    async componentDidMount(): Promise<void> {
        this.setState({
            loading: true,
            selectedOptionState: stateOptions[0],
            selectedOptionType: typeOptions[0],
            selectedOptionId: this.state.idOptions[0],
        });

        await RequestSendingService.sendPostRequestAuthorized(URL.url + "Manager/GetRequests?page=1&pageSize=10000", {
            "RequestTypeId": 0,
            "StateDetailId": 0,
            "Reason": ""
        })
            .then(async response => {
                if (response.status === 200) {
                    const data = await response.json().then(data => data);
                    console.log(data);
                    this.state.rows.pop();
                    data.requests.filter(request => {
                        return request.userSignature[0].nInQueue === 0 && request.stateDetailId === 2 &&
                            request.userSignature[0].approved === false && request.userSignature[0].deleted === false
                    }).forEach(request => {
                        this.state.rows.push({
                            id: request.id,
                            state: StateDetailts[request.stateDetailId],
                            type: RequestTypes[request.requestTypeId],
                            dateFrom: request.dateTimeFrom.slice(0, 10),
                            dateTo: request.dateTimeTo.slice(0, 10),
                            comment: request.reason,
                            details: (request.userSignature.approved) ? "Approved" : "Not approved yet",
                            visible: true
                        })
                        this.state.idOptions.push({
                            value: request.id, label: request.id,
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

        this.setState({
            chosenRequest: this.state.rows.find((element) => element.visible)
        })
    }

    handleChangeDateFrom = selectedOptionDateFrom => {
        this.setState({selectedOptionDateFrom});
    };

    handleChangeDateTo = selectedOptionDateTo => {
        this.setState({selectedOptionDateTo});
    };

    handleChangeState = selectedOptionState => {
        this.setState({selectedOptionState});
    };

    handleChangeId = selectedOptionId => {
        this.setState({selectedOptionId});
    };

    handleChangeType = selectedOptionType => {
        this.setState({selectedOptionType});
    };

    _filter() {
        this.state.rows.forEach(row => {
            let result = true;

            if ((this.state.selectedOptionState.value !== null) && row.state !== StateDetailts[this.state.selectedOptionState.value]) {
                result = result && false;
            } else {
                result = result && true;
            }

            if ((this.state.selectedOptionType.value !== null) && row.type !== RequestTypes[this.state.selectedOptionType.value]) {
                result = result && false;
            } else {
                result = result && true;
            }

            if ((this.state.selectedOptionId.value !== null) && row.id !== this.state.selectedOptionId.value) {
                result = result && false;
            } else {
                result = result && true;
            }

            if (new Date(this.state.selectedOptionDateFrom) > new Date(row.dateFrom) || new Date(this.state.selectedOptionDateTo) < new Date(row.dateTo)) {
                result = result && false;
            } else {
                result = result && true;
            }

            row.visible = result;
        })

        this.setState({chosenRequest: this.state.rows.find((element) => element.visible)})
    }
}