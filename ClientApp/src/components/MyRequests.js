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
    {key: 'state', name: 'state'},
    {key: 'type', name: 'type'},
    {key: 'dateFrom', name: 'dateFrom'},
    {key: 'dateTo', name: 'dateTo'},
    {key: 'comment', name: 'my comment'},
    {key: 'details', name: 'state details'},
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

export class MyRequests extends Component {
    static displayName = MyRequests.name;

    constructor(props) {
        super(props);

        this.state = {
            selectedOptionDateFrom: new Date().setMonth(new Date().getMonth() - 6),
            selectedOptionDateTo: new Date().setMonth(new Date().getMonth() + 2),
            selectedOptionState: null,
            selectedOptionType: null,
            rows: [
                {
                    id: "",
                    state: "loading...",
                    type: 'loading...',
                    dateFrom: "loading...",
                    dateTo: "loading...",
                    comment: "loading...",
                    details: "loading...",
                    visible: true
                },
            ],
            daysUsed: {
                "1": {"days": "Loading..."},
                "2": {"days": "Loading..."},
                "3": {"days": "Loading..."},
                "4": {"days": "Loading..."},
                "5": {"days": "Loading..."},
                "6": {"days": "Loading..."},
                "7": {"days": "Loading..."},
            },
            loading: false,
            error: false,
            chosenRequest: null,
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
                                This year statistics
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Link to="/createRequest" style={{textDecoration: 'none'}}>
                                <Button
                                    outline
                                    block
                                    color="success">
                                    New Request
                                </Button>
                            </Link>
                        </Col>
                        <Col>
                            <center><p><strong>Chosen request: </strong>{"some req"}</p></center>
                        </Col>
                        <Col>
                            <Button
                                // onClick={this._newRequestChangeState}
                                outline
                                block
                                color="info">
                                View Chosen Request
                            </Button>
                        </Col>
                    </Row>
                </Container>
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <center><p><strong>
                                My Requests
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <center><p>Dates From / To</p></center>
                        </Col>
                        <Col>
                            <center><p>State / Type</p></center>
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
                <Container className="mt-3">
                    <Row>
                        <Col>
                            <center><p><strong>
                                This year statistics
                            </strong></p></center>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <p>
                                <strong>Paid holiday: {this.state.daysUsed["1"].days}</strong> days used<br/>
                                <strong>Admin (unpaid) planned: {this.state.daysUsed["2"].days}</strong> days used<br/>
                                <strong>Admin (unpaid) force majeure: {this.state.daysUsed["3"].days}</strong> days used<br/>
                                <strong>Study: {this.state.daysUsed["4"].days}</strong> days used
                            </p>
                        </Col>
                        <Col>
                            <p>
                                <strong>Social: {this.state.daysUsed["5"].days}</strong> days used<br/>
                                <strong>Sick with docs: {this.state.daysUsed["6"].days}</strong> days used<br/>
                                <strong>Sick without docs: {this.state.daysUsed["7"].days}</strong> days used
                            </p>
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
        });
        await RequestSendingService.sendGetRequestAuthorized(URL.url + "Employee/GetDays")
            .then(async response => {
                if (response.status === 200) {
                    const data = await response.json().then(data => data);
                    console.log(data);
                    this.setState({
                        daysUsed: data,
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

        await RequestSendingService.sendPostRequestAuthorized(URL.url + "Employee/GetRequests?page=1&pageSize=10000", {
            "RequestTypeId": 0,
            "StateDetailId": 0,
            "Reason": ""
        })
            .then(async response => {
                if (response.status === 200) {
                    const data = await response.json().then(data => data);
                    console.log(data);
                    this.state.rows.pop();
                    data.requests.forEach(request => {
                        this.state.rows.push({
                            id: request.id,
                            state: StateDetailts[request.stateDetailId],
                            type: RequestTypes[request.requestTypeId],
                            dateFrom: request.dateTimeFrom.slice(0, 10),
                            dateTo: request.dateTimeTo.slice(0, 10),
                            comment: request.reason,
                            details: "Check this in request's page",
                            visible: true
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

            if ((this.state.selectedOptionType.value !== null) && row.state !== RequestTypes[this.state.selectedOptionType.value]) {
                result = result && false;
            } else {
                result = result && true;
            }

            // if (this.state.selectedOptionDateFrom >= Date.parse(row.dateFrom) && this.state.selectedOptionDateTo <= Date.parse(row.dateTo)) {
            //     result = result && false;
            // } else {
            //     result = result && true;
            // }

            row.visible = result;
        })

        this.setState({})

        console.log(this.state.selectedOptionState)
        console.log(this.state.selectedOptionType)
        console.log(this.state.selectedOptionDateFrom)
        console.log(this.state.selectedOptionDateTo)
    }
}