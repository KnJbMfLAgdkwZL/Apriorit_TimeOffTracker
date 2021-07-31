import React, {Component} from 'react';
import {Container, Row, Col, Button} from 'reactstrap';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';
import {RequestSendingService} from "../Services/RequestSendingService";
import {URL} from "../GlobalSettings/URL";
import DataGrid from "react-data-grid";
import {Link} from "react-router-dom";

const columns = [
    {key: 'state', name: 'state'},
    {key: 'type', name: 'type'},
    {key: 'dates', name: 'dates'},
    {key: 'comment', name: 'my comment'},
    {key: 'details', name: 'state details'},
    {key: 'view', name: 'view'}
];

export class MyRequests extends Component {
    static displayName = MyRequests.name;

    constructor(props) {
        super(props);

        this.state = {
            stateOptions: [
                {value: null, label: 'Any'},
            ],
            typeOptions: [
                {value: null, label: 'Any'},
            ],
            selectedOptionDateFrom: new Date().setMonth(new Date().getMonth() - 6),
            selectedOptionDateTo: new Date(),
            selectedOptionState: null,
            selectedOptionType: null,
            rows: [
                {id: "", state: "loading...", type: 'loading...', dates: "loading...", comment: "loading...", details: "loading...", view: "loading...", visible: true},
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
        };
        
        this._filter = this._filter.bind(this);
        this._newRequest = this._newRequest.bind(this);
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
                                    onClick={this._newRequest}
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
                                    dateFormat="yyyy-MM-dd"
                                    selected={this.state.selectedOptionDateFrom}
                                    onChange={this.handleChangeDateFrom}
                                >
                                </DatePicker>
                                <DatePicker
                                    dateFormat="yyyy-MM-dd"
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
                                options={this.state.stateOptions}
                            />
                            <Select
                                value={this.state.selectedOptionType}
                                onChange={this.handleChangeType}
                                options={this.state.typeOptions}
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
            selectedOptionState: this.state.stateOptions[0],
            selectedOptionType: this.state.typeOptions[0],
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
    
    _newRequest() {

    }
    
    _filter() {
        console.log(this.state.selectedOptionState)
        console.log(this.state.selectedOptionType)
        console.log(this.state.selectedOptionDateFrom)
        console.log(this.state.selectedOptionDateTo)
    }
}