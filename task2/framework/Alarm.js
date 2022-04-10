import * as Notifications from 'expo-notifications';

import AppDefines from '../defines/AppDefines';

class Alarm {

    static fromDate({ date, radio }) {
        alarm = new Alarm();
        alarm.hours = date.getHours();
        alarm.minutes = date.getMinutes();
        alarm.radioName = radio.title;
        alarm.radioUri = radio.uri;
        alarm.notifyId = '';
        alarm.bIsActive = false;

        return alarm;
    }

    static fromJson({ jsonObject }) {
        alarm = new Alarm();
        alarm.hours = jsonObject.hours;
        alarm.minutes = jsonObject.minutes;
        alarm.radioName = jsonObject.radioName;
        alarm.radioUri = jsonObject.radioUri;
        alarm.notifyId = jsonObject.notifyId;
        alarm.bIsActive = jsonObject.bIsActive;

        return alarm;
    }

    getDate() {
        result = new Date();
        result.setHours(this.hours);
        result.setMinutes(this.minutes);
        result.setSeconds(0);

        // Ensure the date is in future
        if (result < new Date()) {
            result = new Date(result.getTime() + 86400000);
        }

        return result;
    }

    async activate() {
        this.bIsActive = true;
        this.notifyId = await Notifications.scheduleNotificationAsync({
            content: {
                title: AppDefines.text.titleNotification,
                data: {
                    radioUri: this.radioUri,
                    radioName: this.radioName,
                },
            },
            trigger: {
                hour: this.hours,
                minute: this.minutes,
                repeats: true,
            },
        });
    }

    async deactivate() {
        this.bIsActive = false;
        try {
            await Notifications.cancelScheduledNotificationAsync(this.notifyId);
        } catch (e) {
            console.log("[Alarm] Could not cancel notification with " + e)
        }
        this.notifyId = '';
    }

    isActive() {
        return this.bIsActive;
    }
}

export default Alarm;
