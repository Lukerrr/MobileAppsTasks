import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:path_provider/path_provider.dart';
import 'package:todolist/framework/user_task.dart';

class TaskManager {

    TaskManager._internal() {
        WidgetsFlutterBinding.ensureInitialized();
    }

    factory TaskManager() => _instance;

    int addTask(UserTask task) {
        final taskId = getTasksNum();
        _tasksList.add(task);
        save();
        return taskId;
    }

    UserTask getTask(int taskId) {
        return _tasksList[taskId];
    }

    void removeTask(int taskId) {
        _tasksList.removeAt(taskId);
        save();
    }

    void updateTask(int taskId, UserTask task) {
        _tasksList[taskId] = task;
        save();
    }

    int getTasksNum() {
        return _tasksList.length;
    }

    void save() async {
        Map<String, dynamic> jsonData = {
            'tasks': _tasksList.map((e) => e.toJson()).toList()
        };

        String jsonString = jsonEncode(jsonData);

        final tasksFile = await _getTasksFile();
        tasksFile.writeAsStringSync(jsonString);
    }

    void load(void Function(bool) onLoadedCb) async {
        bool bResult = true;

        try {
            final tasksFile = await _getTasksFile();
            final jsonStringTasks = await tasksFile.readAsString();
            final jsonDataTasks = jsonDecode(jsonStringTasks);

            _tasksList = List<UserTask>.from(
                jsonDataTasks['tasks'].map(
                        (e) => UserTask.fromJson(e)
                )
            );
        } catch (e) {
            bResult = false;
        }

        onLoadedCb(bResult);
    }

    Future<File> _getTasksFile() async {
        final dir = await getApplicationDocumentsDirectory();
        final filePath = dir.path;
        final tasksFile = File(filePath + '/' + saveFileName);
        if (!tasksFile.existsSync()) {
            tasksFile.createSync(recursive: true);
        }
        return tasksFile;
    }

    List<UserTask> _tasksList = [];

    static const String saveFileName = "tasks.json";

    static final TaskManager _instance = TaskManager._internal();
}
