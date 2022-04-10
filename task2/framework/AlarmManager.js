import * as FS from 'expo-file-system';

import Alarm from '../framework/Alarm';

class AlarmManager {

    addAlarm({ alarm }) {
        id = this.alarmsList.length;
        this.alarmsList.push(alarm);
        this.save();
        return id;
    }

    removeAlarm({ id }) {
        alarm = this.getAlarm({ id: id });
        if (alarm.isActive()) {
            alarm.deactivate();
        }
        this.alarmsList.splice(id, 1);
        this.save();
    }

    getAlarms() {
        return this.alarmsList;
    }

    getAlarmsNum() {
        return this.alarmsList.length;
    }

    getAlarm({ id }) {
        return this.alarmsList[id];
    }

    async save() {
        jsonData = JSON.stringify(this.alarmsList);
        saveFilePath = FS.documentDirectory + '/' + this.saveFileName;

        await FS.writeAsStringAsync(saveFilePath, jsonData);
    }

    async load({ onLoadedCb }) {
        saveFilePath = FS.documentDirectory + '/' + this.saveFileName;
        this.alarmsList = [];

        FS.readAsStringAsync(saveFilePath)
            .then((jsonData) => {
                alarmJsonArray = JSON.parse(jsonData);
                alarmJsonArray.map((alarmJson) => {
                    this.alarmsList.push(Alarm.fromJson({ jsonObject: alarmJson }));
                });
                onLoadedCb(true);
            })
            .catch((err) => {
                this.alarmsList = [];
                onLoadedCb(false);
            });
    }

    alarmsList = [];

    static saveFileName = "alarms.json";
    static instance = AlarmManager.instance || new AlarmManager();
}

export default AlarmManager;
