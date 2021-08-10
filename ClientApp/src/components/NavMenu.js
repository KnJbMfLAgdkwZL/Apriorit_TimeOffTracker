import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import {AuthService} from "../Services/AuthService";

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/">TimeOffTracker</NavbarBrand>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow">
                {AuthService.isLogged() && AuthService.getCurrentUserRole() === "Admin" &&
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/">Users</NavLink>
                  </NavItem>
                }
                {AuthService.isLogged() && (AuthService.getCurrentUserRole() === "Employee" || AuthService.getCurrentUserRole() === "Manager") &&
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/">My Requests</NavLink>
                </NavItem>
                }
                {AuthService.isLogged() && (AuthService.getCurrentUserRole() === "Employee" || AuthService.getCurrentUserRole() === "Manager") &&
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/createRequest">Create New</NavLink>
                </NavItem>
                }
                {AuthService.isLogged() && (AuthService.getCurrentUserRole() === "Accounting" || AuthService.getCurrentUserRole() === "Manager") &&
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/othersRequests">Others' Requests</NavLink>
                </NavItem>
                }
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/auth">Me</NavLink>
                </NavItem>
              </ul>
            </Collapse>
          </Container>
        </Navbar>
      </header>
    );
  }
}
