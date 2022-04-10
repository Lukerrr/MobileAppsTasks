import * as React from 'react';

import { View } from 'react-native';

import { Button, Headline } from 'react-native-paper';

import AppDefines from '../defines/AppDefines';

class AlarmNotify extends React.Component {
    render() {
        return (

            <View style={
                {
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    height: 250,
                    margin: 15,
                    padding: 20,
                    borderRadius: 15,
                    backgroundColor: this.props.theme.colors.surface
                }
            }>
                <Headline style={
                    {
                        fontSize: 32,
                    }
                }>
                    {AppDefines.text.titleNotification}
                </Headline>
                <View style={
                    {
                        flexDirection: 'column',
                        alignItems: 'center',
                        justifyContent: 'center',
                    }
                }>
                    <Headline style={
                        {
                            fontSize: 24,
                        }
                    }>
                        {AppDefines.text.labelPlayingRadio}
                    </Headline>
                    <Headline style={
                        {
                            fontSize: 18,
                        }
                    }>
                        {this.props.radioName}
                    </Headline>
                </View>
                <Button
                    mode="contained"
                    onPress={this.props.onHide}
                    style={
                        {
                            alignContent: 'flex-end'
                        }
                }>
                    {AppDefines.text.labelDisableAlarm}
                </Button>
            </View>
        );
    }
}

export default AlarmNotify;
