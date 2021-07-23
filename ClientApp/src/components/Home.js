import React, { Component } from 'react';
import '../grid.css'
export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
        <main>
            <div class="dropdown">
                <label>Name</label>
                <button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    Dropdown button 1
                </button>
                <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
                    <a class="dropdown-item" href="#">1</a>
                    <a class="dropdown-item" href="#">2</a>
                    <a class="dropdown-item" href="#">3</a>
                </div>
                <label>Role</label>
                <button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    Dropdown button 2
                </button>
                <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
                    <a class="dropdown-item" href="#">1</a>
                    <a class="dropdown-item" href="#">2</a>
                    <a class="dropdown-item" href="#">3</a>
                </div>
                <button variant="primary">Filter</button>

            </div>
    <div class="row mb-3">
      <div class="col-4 themed-grid-col"></div>
      <div class="col-4 themed-grid-col"></div>
      <div class="col-4 themed-grid-col"></div>
    </div>

    <div class="row mb-3">
      <div class="col-sm-4 themed-grid-col"></div>
      <div class="col-sm-4 themed-grid-col"></div>
      <div class="col-sm-4 themed-grid-col"></div>
    </div>

    <div class="row mb-3">
      <div class="col-md-4 themed-grid-col"></div>
      <div class="col-md-4 themed-grid-col"></div>
      <div class="col-md-4 themed-grid-col"></div>
    </div>

    <div class="row mb-3">
      <div class="col-lg-4 themed-grid-col"></div>
      <div class="col-lg-4 themed-grid-col"></div>
      <div class="col-lg-4 themed-grid-col"></div>
    </div>

    <div class="row mb-3">
      <div class="col-xl-4 themed-grid-col"></div>
      <div class="col-xl-4 themed-grid-col"></div>
      <div class="col-xl-4 themed-grid-col"></div>
    </div>

    <div class="row mb-3">
      <div class="col-xxl-4 themed-grid-col"></div>
      <div class="col-xxl-4 themed-grid-col"></div>
      <div class="col-xxl-4 themed-grid-col"></div>
    </div>
     </main>
    );
  }
}
