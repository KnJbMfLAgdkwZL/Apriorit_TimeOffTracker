import React, {Component} from 'react';
import {Route} from "react-router-dom";
import {Redirect} from 'react-router'
import {Layout} from './components/Layout';
import {Home} from './components/Home';
import {Authorization} from "./components/Authorization";
import {AuthService} from "./Services/AuthService";

import './custom.css'

export default class App extends Component {
    static displayName = App.name;
    
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Layout>
                <Route exact path="/" component={Home}>
                    {!AuthService.isLogged() ? <Redirect to="/auth">Protected Page</Redirect> : <Home/>}
                </Route>
                <Route path='/auth' component={Authorization}/>
            </Layout>
        );
    }
} 
