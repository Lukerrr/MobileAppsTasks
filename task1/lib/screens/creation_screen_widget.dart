import 'dart:io';

import 'package:flutter/material.dart';
import 'package:todolist/framework/app_colors.dart';
import 'package:todolist/framework/app_text.dart';
import 'package:todolist/framework/user_task.dart';
import 'package:todolist/components/task_edit_widget.dart';
import 'package:todolist/screens/home_screen_widget.dart';

class CreationScreenController extends ChangeNotifier {

    void setConstructedTask(UserTask task) {
        this.task = task;
        notifyListeners();
    }

    UserTask? task = null;
}

class CreationScreenWidget extends StatefulWidget {

    CreationScreenWidget({required this.controller});

    @override
    _CreationScreenWidgetState createState() => _CreationScreenWidgetState();

    CreationScreenController controller;
}

class _CreationScreenWidgetState extends State<CreationScreenWidget> {

    _CreationScreenWidgetState() {
        _editWidgetController.addListener(() {
            setState(() {
                _bCanSaveTask = _editWidgetController.hasValidData();
            });
        });
    }

    void _onConstructWidget() {
        if (_editWidgetController.hasValidData()) {
            widget.controller.setConstructedTask(
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
                        AppText.titleCreationScreen,
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
                                    ? _onConstructWidget
                                    : null,
                                child: Text(
                                    AppText.labelButtonCreationAccept,
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
    bool _bCanSaveTask = false;
}
