import * as React from 'react';

import { View } from 'react-native';

import { Appbar, Button, Headline, List } from 'react-native-paper';
import { TimePickerModal } from 'react-native-paper-dates';

import Moment from 'moment';

import Alarm from '../framework/Alarm';
import Radio from '../framework/Radio';
import AppDefines from '../defines/AppDefines';

class CreationScreen extends React.Component {

    constructor() {
        super();
        this.radioList = [
            new Radio({ icon: "violin", title: "CalmRadio - Vivaldi", uri: "http://23.82.11.87:8928/stream" }),
            new Radio({ icon: "skull", title: "Metal Live Radio", uri: "http://51.255.8.139:8738/stream" }),
            new Radio({ icon: "music-note-eighth", title: "Hits Of Bollywood", uri: "http://198.50.156.92:8255/stream" }),
        ];
        this.state.radio = this.radioList[0];
    }

    state = {
        time: new Date(),
        radio: null,
        timeOpen: false,
        radioListExpanded: false,
    }

    _onAlarmDeclined() {
        this.props.navigation.goBack();
    }

    _onAlarmConfirmed() {
        this.props.navigation.navigate("Home", ({
            newAlarm: Alarm.fromDate({
                date: this.state.time,
                radio: this.state.radio
            })
        }));
    }

    _onPickTimeRequest() {
        this.state.timeOpen = true;
        this.setState(this.state);
    }

    _onPickTimeConfirm({ hours, minutes }) {
        this.state.time.setHours(hours);
        this.state.time.setMinutes(minutes);
        this._onPickTimeDismiss();
    }

    _onPickTimeDismiss() {
        this.state.timeOpen = false;
        this.setState(this.state);
    }

    _onPickRadioExpand() {
        this.state.radioListExpanded = !this.state.radioListExpanded;
        this.setState(this.state);
    }

    _onPickRadio({ id }) {
        this.state.radio = this.radioList[id];
        this._onPickRadioExpand();
    }

    render() {
        return (
            <View style={
                {
                    flex: 1,
                    backgroundColor: this.props.theme.colors.background,
                }
            }>
                <Appbar.Header>
                    <Appbar.Action icon="arrow-left" onPress={this._onAlarmDeclined.bind(this)} />
                    <Appbar.Content title={AppDefines.text.titleCreateionScreen} />
                    <Appbar.Action icon="check" onPress={this._onAlarmConfirmed.bind(this)} />
                </Appbar.Header>
                <View style={
                    {
                        flex: 1,
                        backgroundColor: this.props.theme.colors.background,
                        flexDirection: 'column',
                        alignItems: 'stretch',
                    }
                }>
                    <Button
                        mode="text"
                        color={this.props.theme.colors.text}
                        onPress={this._onPickTimeRequest.bind(this)}
                        style={
                        {
                            padding: 25,
                            alignSelf: 'stretch',
                        }
                    } labelStyle={
                        {
                            fontSize: 72,
                        }
                    }>
                        {Moment(this.state.time).format('HH:mm')}
                    </Button>
                    <Headline style={
                        {
                            fontSize: 24,
                            lineHeight: 32,
                            marginTop: 20,
                            marginLeft: 20,
                            textAlign: 'left',
                        }
                    }>
                        {AppDefines.text.titleRadioChoose}
                    </Headline>
                    <List.Accordion
                        title={this.state.radio.title}
                        expanded={this.state.radioListExpanded}
                        onPress={this._onPickRadioExpand.bind(this)}
                        left={props => <List.Icon {...props} icon={this.state.radio.icon} />}
                    >
                        {
                            this.radioList.map(
                                (radio, i) => (
                                    <List.Item
                                        key={i}
                                        title={radio.title}
                                        onPress={() => {
                                            this._onPickRadio({ id: i });
                                        }}
                                        left={() => <List.Icon icon={radio.icon} />}
                                        style={
                                            {
                                                marginLeft: 15,
                                            }
                                        }
                                    />
                                )
                            )
                        }
                    </List.Accordion>
                </View>

                <TimePickerModal
                    locale="en-US"
                    visible={this.state.timeOpen}
                    onDismiss={this._onPickTimeDismiss.bind(this)}
                    onConfirm={this._onPickTimeConfirm.bind(this)}
                    hours={this.state.time.hours}
                    minutes={this.state.time.minutes}
                />

            </View>
        );
    }
}

export default CreationScreen;

