require('./assets/index.js');
import React, {Component} from 'react';
import ReactDOM from 'react-dom';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import getMuiTheme from 'material-ui/styles/getMuiTheme';
import ClassifierComponent from './components/ClassifierComponent'

const muiTheme = getMuiTheme({
    isRtl:true,
});

ReactDOM.render(
    <MuiThemeProvider muiTheme={muiTheme}>
        <div className="container center-xs">
            <ClassifierComponent/>

            {/*<div className="row rightAlign bottom-xs">
                <div className="col-xs-4">
                    <Headline/>
                </div>
                <div className="col-xs-4">
                </div>
                <div className="col-xs-4 center-xs bottom-xs">
                    <VersionSelector/>
                </div>
            </div>
            <div className="row">
                <div className="col-xs-12">
                    <MyStatefulEditor/>
                </div>
            </div>
            <ActionButtons/>*/}
        </div>
    </MuiThemeProvider>,
    document.getElementById('app')
);