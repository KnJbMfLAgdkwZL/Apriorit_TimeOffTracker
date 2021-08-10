import React, {Component} from 'react';
import {Route, Switch} from "react-router-dom";
import {Redirect} from 'react-router'
import {Layout} from './components/Layout';
import {Authorization} from "./components/Authorization";
import {AuthService} from "./Services/AuthService";

import {Users} from "./components/Users";
import {CreateUser} from "./components/additionalComponents/CreateUser";
import {MyRequests} from "./components/MyRequests";
import {Request} from "./components/additionalComponents/Request";
import {CreateRequest} from "./components/additionalComponents/CreateRequest";
import {OthersRequests} from "./components/OthersRequests";
import {ApproveRequest} from "./components/additionalComponents/ApproveRequest";
import {RejectRequest} from "./components/additionalComponents/RejectRequest";


export default class App extends Component {
    static displayName = App.name;
    
    constructor(props) {
        super(props);
        
        this.state = {
            currentUserRole: AuthService.getCurrentUserRole()
        }
    }

    render() {
        switch (this.state.currentUserRole) {
            case "Admin": {
                return (
                    <AdminComponent/>
                );
            }
            case "Employee": {
                return (
                    <EmployeeComponent/>
                );
            }
            case "Manager": {
                return (
                    <ManagerComponent/>
                );
            }
            default: {
                return (
                    <Layout>
                        {!AuthService.isLogged() && <Redirect to="/auth">Protected Page</Redirect>}
                        <Route exact path='/auth' component={Authorization}/>
                        <Route>
                            <Redirect to="/auth"/>
                        </Route>
                    </Layout>
                );
            }
        }

    }
} 

class AdminComponent extends Component {
    static displayName = AdminComponent.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Layout>
                <Route exact path="/" component={Users}>
                    {!AuthService.isLogged() && <Redirect to="/auth">Protected Page</Redirect>}
                </Route> 
                <Route exact path='/auth' component={Authorization}/>
                <Route exact path='/createUser' component={CreateUser}/>
                <Route>
                    <Redirect to="/"/>
                </Route>
            </Layout>
        );
    }
}

class EmployeeComponent extends Component {
    static displayName = EmployeeComponent.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Layout>
                <Switch>
                    <Route exact path="/" component={MyRequests}>
                        {!AuthService.isLogged() && <Redirect to="/auth">Protected Page</Redirect>}
                    </Route>
                    <Route exact path='/auth' component={Authorization}/>
                    <Route exact path='/createRequest' component={CreateRequest}/>
                    <Route path='/request' component={Request}/>
                    <Route>
                        <Redirect to="/"/>
                    </Route>
                </Switch>
            </Layout>
        );
    }
}

class ManagerComponent extends Component {
    static displayName = ManagerComponent.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Layout>
                <Switch>
                    <Route exact path="/" component={MyRequests}>
                        {!AuthService.isLogged() && <Redirect to="/auth">Protected Page</Redirect>}
                    </Route>
                    <Route exact path='/auth' component={Authorization}/>
                    <Route exact path='/createRequest' component={CreateRequest}/>
                    <Route path='/request' component={Request}/>
                    <Route path='/othersRequests' component={OthersRequests}/>
                    <Route path='/approveRequest' component={ApproveRequest}/>
                    <Route path='/rejectRequest' component={RejectRequest}/>
                    <Route>
                        <Redirect to="/"/>
                    </Route>
                </Switch>
            </Layout>
        );
    }
}
