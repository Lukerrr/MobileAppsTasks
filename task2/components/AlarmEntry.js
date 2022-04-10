import * as React from 'react';

import { View } from 'react-native';
import { IconButton, Headline, Switch } from 'react-native-paper';

import Moment from 'moment';

class AlarmEntry extends React.Component {

    _onAlarmRemove() {
        this.props.onAlarmRemove({ id: this.props.id });
    }

    _onSwitchAlarmStatus() {
        if (this.props.alarm.isActive()) {
            this.props.alarm.deactivate();
        } else {
            this.props.alarm.activate();
        }
        this.props.onAlarmStatusChange({ id: this.props.id });
        this.setState(this.props);
	}

    render() {
		return (
            <View style={
                {
                    flex: 1,
                    flexDirection: 'row',
                    alignItems: 'center',
                    justifyContent: 'center',
                    height: 100,
                    margin: 10,
                    marginBottom: 0,
                    borderRadius: 20,
                    borderWidth: 2,
                    borderColor: this.props.theme.colors.disabled,
                }
            }>
                <View style={
                    {
                        flex: 1,
                        flexDirection: 'row',
                        alignItems: 'center',
                        justifyContent: 'flex-start',
                        marginLeft: 10,
                    }
                }>
                    <IconButton icon="close" size={24} onPress={this._onAlarmRemove.bind(this)} />
                    <View style={{ width: 10 }} />
                    <Headline style={
                        {
                            fontSize: 32,
                            lineHeight: 38,
                        }
                    }>
                        {Moment(this.props.alarm.getDate()).format('HH:mm')}
                    </Headline>
                </View>
                <View style={
                    {
                        flex: 1,
                        flexDirection: 'row',
                        alignItems: 'center',
                        justifyContent: 'flex-end',
                    }
                }>
                    <Switch
                        value={this.props.alarm.isActive()}
                        onValueChange={this._onSwitchAlarmStatus.bind(this)}
                        style={
                            {
                                marginRight: 15,
                                transform: [{ scaleX: 1.25 }, { scaleY: 1.25 }]
							}
                        }
                    />
                </View>
            </View>
		);
	}

}

export default AlarmEntry;
