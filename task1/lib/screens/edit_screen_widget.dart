import 'dart:io';

import 'package:flutter/material.dart';
import 'package:todolist/framework/app_colors.dart';
import 'package:todolist/framework/app_text.dart';
import 'package:todolist/framework/user_task.dart';
import 'package:todolist/components/task_edit_widget.dart';
import 'package:todolist/screens/home_screen_widget.dart';

class EditScreenController extends ChangeNotifier {

    EditScreenController({required this.task});

    void setTask(UserTask task) {
        this.task = task;
        notifyListeners();
    }

    UserTask task;
}

class EditScreenWidget extends StatefulWidget {

    EditScreenWidget({required this.controller});

    @override
    _EditScreenWidgetState createState() => _EditScreenWidgetState(controller.task);

    EditScreenController controller;
}

class _EditScreenWidgetState extends State<EditScreenWidget> {

    _EditScreenWidgetState(UserTask task) {
        _editWidgetController.setName(task.name);
        _editWidgetController.setNotification(task.notificationTime);

        _editWidgetController.addListener(() {
            setState(() {
                _bCanSaveTask = _editWidgetController.hasValidData();
            });
        });
    }

    void _onConfirmWidget() {
        if (_editWidgetController.hasValidData()) {
            widget.controller.setTask(
                UserTask(
                    name: _editWidgetController.textName,
                    notificationTime: _editWidgetController.dateNotification,
                )
            );
        }
        _onLeave();
    }

    void _onLeave() {
        Navigator.pushNamed(context, HomeScreenWidget.id);
    }

    @override
    Widget build(BuildContext context) {
        return WillPopScope(
            onWillPop: () async { exit(0); },
            child: Scaffold(

                appBar: AppBar(

                    title: Text(
                        AppText.titleEditScreen,
                        style: TextStyle(
                            color: AppColors.textTitles
                        ),
                    ),
                    backgroundColor: AppColors.primary,
                    automaticallyImplyLeading: false,

                ),

                body: TaskEditWidget(controller: _editWidgetController),

                bottomNavigationBar: Container(
                    height: 60,

                    decoration: BoxDecoration(
                        color: AppColors.primary
                    ),


                    child: Row(
                        children: [

                            ElevatedButton(
                                onPressed: _bCanSaveTask
                                    ? _onConfirmWidget
                                    : null,
                                child: Text(
                                    AppText.labelButtonEditAccept,
                                    style: TextStyle(
                                        color: AppColors.textTitles,
                                    ),
                                    textScaleFactor: 1.2,
                                ),
                                style: ElevatedButton.styleFrom(
                                    primary: AppColors.secondary,
                                    fixedSize: Size(200, 40),
                                ),
                            ),

                            ElevatedButton(
                                onPressed: _onLeave,
                                child: Text(
                                    AppText.labelButtonDecline,
                                    style: TextStyle(
                                        color: AppColors.textTitles,
                                    ),
                                    textScaleFactor: 1.2,
                                ),
                                style: ElevatedButton.styleFrom(
                                    primary: AppColors.secondary,
                                    fixedSize: Size(150, 40),
                                ),
                            ),

                        ],
                        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                    ),
                ),
            ),
        );
    }

    TaskEditWidgetController _editWidgetController = TaskEditWidgetController();
    bool _bCanSaveTask = true;
}
