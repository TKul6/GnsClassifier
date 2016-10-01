/**
 * Created by erang on 9/30/2016.
 */
import React, {Component} from 'react';
import RaisedButton from 'material-ui/RaisedButton'
import $ from 'jquery';

export default class ClassifierComponent extends Component {
    constructor(props) {
        super(props);
        this.submitSafeWord = this.submitSafeWord.bind(this);
        this.submitUnsafeWord = this.submitUnsafeWord.bind(this);
        this.submitWord = this.submitWord.bind(this);
        this.getNewWord = this.getNewWord.bind(this);
        this.state = {
            currentWord: ""
        }
    }

    submitUnsafeWord(){
        this.submitWord(this.state.currentWord, 0);
    }

    submitSafeWord(){
        this.submitWord(this.state.currentWord, 1);
    }

    submitWord(word, result){
        var address = `http://localhost:8700/GnsClassifierService/SubmitResult?word=${word}&result=${result}`;
        //$.get(address);
        $.ajax({
            type: "GET",
            url: address,
            dataType: "json",
            processData: false,
            xhrFields: {
                withCredentials: true
            },
        });
        this.getNewWord();
    }

    getNewWord(){
        $.ajax({
            type: "GET",
            url: "http://localhost:8700/GnsClassifierService/GetWordToClassify",
            dataType: "json",
            processData: false,
            xhrFields: {
                withCredentials: true
            },
            success: function(result) { this.setState({
                currentWord: result,
            });}.bind(this)
        });
    }

    render() {
        let buttonStyle = { margin: '10px', width: '48%', height: '80px'}
        let buttonLabelStyle = {fontSize: '30px', verticalAlign: 'middle', justifyContent: 'center', textTransform: 'none', lineHeight: '80px'}
        return (
            <div>
                <div className="classifierDiv row center-xs ">
                    <label className="titleText">GNS Classifier</label>
                </div>
                <div className="classifierDiv row center-xs">
                    <label className="classifierWord">{this.state.currentWord}</label>
                </div>
                <div className="classifierDiv row center-xs">
                    <RaisedButton style={buttonStyle}
                                  label="Unsafe"
                                  labelColor="white"
                                  labelStyle={buttonLabelStyle}
                                  onTouchTap={this.submitUnsafeWord}
                                  backgroundColor="red"/>
                    <RaisedButton style={buttonStyle}
                                  label="Safe"
                                  labelColor="white"
                                  labelStyle={buttonLabelStyle}
                                  onTouchTap={this.submitSafeWord}
                                  backgroundColor="green"/>
                </div>
                <div className="classifierDiv row center-xs">
                    <RaisedButton style={buttonStyle}
                                  label="Get a New Word"
                                  fullWidth={true}
                                  labelStyle={buttonLabelStyle}
                                  onTouchTap={this.getNewWord}
                                  primary={true}/>
                </div>
            </div>

        )
    }
}
