import React, {Component} from 'react';
import {Container, Row, Col, Button} from 'reactstrap';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';

const options = [
    { value: 'chocolate', label: 'Chocolate' },
    { value: 'strawberry', label: 'Strawberry' },
    { value: 'vanilla', label: 'Vanilla' },
];


export class MyRequests extends Component {
    static displayName = MyRequests.name;

    constructor(props) {
        super(props);

        this.state = {
            selectedOption: null
        };
    }

    handleChange = selectedOption => {
        this.setState({ selectedOption });
    };
    
    render() {
        return (
            <div>
                <Container>
                    <Row>
                        <Col>
                            <p><strong>
                                <center>Overall statistics</center>
                            </strong></p>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <p>
                                Paid holiday: {} days used<br/>
                                Admin (unpaid) planned: {} days used<br/>
                                Admin (unpaid) force majeure: {} days used
                            </p>
                        </Col>
                        <Col>
                            <p>
                                Sick with docs: {} days used<br/>
                                Sick without docs: {} days used
                            </p>
                        </Col>
                        <Col>
                            <Button
                                outline
                                color="success">
                                New Request
                            </Button>
                        </Col>
                    </Row>
                </Container>
                <Container>
                    <Row>
                        <Col>
                            <p><strong>
                                <center>My Requests</center>
                            </strong></p>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <DatePicker
                                selected={new Date()}
                            >
                            </DatePicker>
                            <DatePicker
                                selected={new Date()}
                                className="mt-2"
                            >
                            </DatePicker>
                        </Col>
                        <Col>
                            <Select
                                value={this.state.selectedOption}
                                onChange={this.handleChange}
                                options={options}
                            />
                            <Select
                                value={this.state.selectedOption}
                                onChange={this.handleChange}
                                options={options}
                                className="mt-2"
                            />
                        </Col>
                        <Col>
                            <Button
                                fullWidth
                                outline
                                color="info">
                                Apply Filter
                            </Button>
                        </Col>
                    </Row>
                    <Row>
                        <Col>

                        </Col>
                    </Row>
                </Container>
            </div>
        );
    }
}