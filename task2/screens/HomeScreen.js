import * as React from 'react';

import { View, ScrollView } from 'react-native';

import { Appbar } from 'react-native-paper';

import AlarmEntry from '../components/AlarmEntry';
import AppDefines from '../defines/AppDefines';
import AlarmManager from '../framework/AlarmManager';

class HomeScreen extends React.Component {

    constructor() {
        super();

        this.state.alarmManager.load({
            onLoadedCb: (bResult) => {
                this.setState(this.state);
            }
        });
    }

    state = {
        alarmManager: AlarmManager.instance
    }

    _onAddAlarmPressed() {
        this.props.navigation.navigate("Creation");
    }

    _onAlarmRemove({ id }) {
        this.state.alarmManager.removeAlarm({ id: id });
        this.setState(this.state);
    }

    _onAlarmStatusChange({ id }) {
        this.state.alarmManager.save();
    }

    componentDidUpdate() {
        if (this.props.route.params && this.props.route.params.newAlarm != null) {
            this.state.alarmManager.addAlarm({ alarm: this.props.route.params.newAlarm });
            this.props.route.params.newAlarm = null;
            this.setState(this.state);
        }
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
                    <Appbar.Content title={AppDefines.text.titleHomeScreen} />
                    <Appbar.Action icon="plus" onPress={this._onAddAlarmPressed.bind(this)} />
                </Appbar.Header>
                <ScrollView>
                    {
                        this.state.alarmManager.getAlarms().map(
                            (alarm, i) => (
                                <AlarmEntry
                                    key={i}
                                    id={i}
                                    alarm={alarm}
                                    theme={this.props.theme}
                                    onAlarmRemove={this._onAlarmRemove.bind(this)}
                                    onAlarmStatusChange={this._onAlarmStatusChange.bind(this)}
                                />
                            )
                        )
                    }
                </ScrollView>
            </View>
        );
    }
}

export default HomeScreen;
