import 'dart:io';

import 'package:flutter/material.dart';
import 'package:loading_animations/loading_animations.dart';
import 'package:todolist/framework/app_colors.dart';
import 'package:todolist/framework/app_text.dart';
import 'package:todolist/framework/notify_manager.dart';
import 'package:todolist/framework/task_manager.dart';
import 'package:todolist/framework/user_task.dart';
import 'package:todolist/components/task_entry_widget.dart';
import 'package:todolist/screens/creation_screen_widget.dart';
import 'package:todolist/screens/edit_screen_widget.dart';

class HomeScreenWidget extends StatefulWidget {

    @override
    _HomeScreenWidgetState createState() => _HomeScreenWidgetState();

    static const String id = "home-screen";
}

class _HomeScreenWidgetState extends State<HomeScreenWidget> {

    void _onTaskRemove(int taskId) {
        NotifyManager.removeNotification(id: taskId);

        setState(() {
            TaskManager().removeTask(taskId);
        });
    }

    void _onTaskStateChanged(int taskId, bool state) {
        UserTask task = TaskManager().getTask(taskId);
        task.bIsDone = state;
        TaskManager().updateTask(taskId, task);
    }

    void _onTaskEdit(BuildContext context, int taskId) {
        UserTask task = TaskManager().getTask(taskId);

        var editController = EditScreenController(task: task);

        editController.addListener(() {

            NotifyManager.removeNotification(id: taskId);
            _addTaskNotification(taskId);

            setState(() {
                TaskManager().updateTask(taskId, editController.task);
            });
        });

        Navigator.push(
            context,
            MaterialPageRoute(
                builder: (BuildContext routeBuildContext) {
                    return EditScreenWidget(controller: editController);
                }
            ),
        );
    }

    void _onOpenCreationWidget(BuildContext context) {
        var creationController = CreationScreenController();

        creationController.addListener(() {
            var taskId = TaskManager().addTask(creationController.task!);
            _addTaskNotification(taskId);
            setState(() {
            });
        });

        Navigator.push(
            context,
            MaterialPageRoute(
                builder: (BuildContext routeBuildContext) {
                    return CreationScreenWidget(controller: creationController);
                }
            ),
        );
    }

    void _onTasksLoaded(bResult) {
        if (bResult) {
            for (var i = 0; i < TaskManager().getTasksNum(); ++i) {
                _addTaskNotification(i);
            }
        }

        setState(() {
            bTasksLoaded = true;
        });
    }

    void _addTaskNotification(int taskId) {
        UserTask task = TaskManager().getTask(taskId);
        if (task.notificationTime != null) {
            NotifyManager.planNotification(
                id: taskId,
                title: AppText.titleNotification,
                body: task.name,
                date: task.notificationTime!,
            );
        }
    }

    @override
    Widget build(BuildContext context) {
        if (!bTasksLoaded) {
            TaskManager().load(
                _onTasksLoaded
            );
        }

        Widget loadingWidget = Container(
            child: Column(
                children: [
                    SizedBox(
                        height: 40,
                    ),
                    LoadingBouncingGrid.square(
                        backgroundColor: AppColors.primary,
                    ),
                    SizedBox(
                        height: 40,
                        child: Text(
                            AppText.labelLoading,
                            style: TextStyle(
                                decoration: TextDecoration.none,
                                color: AppColors.primary,
                                fontSize: 18,
                            ),
                        ),
                    ),
                ],
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                crossAxisAlignment: CrossAxisAlignment.center,
            ),
            color: AppColors.background,
        );

        Widget appWidget = Scaffold(

            appBar: AppBar(

                title: Text(
                    AppText.titleHomeScreen,
                    style: TextStyle(
                        color: AppColors.textTitles
                    ),
                ),
                backgroundColor: AppColors.primary,
                automaticallyImplyLeading: false,

            ),

            body: Padding(
                padding: EdgeInsets.symmetric(
                    horizontal: 10.0,
                    vertical: 20.0,
                ),
                child: ListView.separated(
                    itemCount: TaskManager().getTasksNum(),
                    itemBuilder: (context, taskId) {
                        UserTask task = TaskManager().getTask(taskId);
                        return Container(
                            decoration: BoxDecoration(
                                color: AppColors.background,
                                border: Border.all(
                                    color: AppColors.background,
                                    width: 10,
                                ),
                                borderRadius: BorderRadius.all(
                                    Radius.circular(20)
                                ),
                            ),
                            child: TaskEntryWidget(
                                taskId: taskId,
                                task: task,
                                onStateChangedCb: _onTaskStateChanged,
                                onEditCb: (taskId) {
                                    _onTaskEdit(context, taskId);
                                },
                                onRemoveCb: _onTaskRemove,
                            ),
                        );
                    },
                    separatorBuilder: (context, taskId) {
                        return SizedBox(
                            height: 20,
                        );
                    },
                ),
            ),

            bottomNavigationBar: Container(
                height: 60,

                decoration: BoxDecoration(
                    color: AppColors.primary
                ),

                child: Center(

                    child: ElevatedButton(
                        onPressed: () {
                            _onOpenCreationWidget(context);
                        },
                        child: Text(
                            AppText.labelButtonAddTask,
                            style: TextStyle(
                                color: AppColors.textTitles,
                            ),
                            textScaleFactor: 1.2,
                        ),
                        style: ElevatedButton.styleFrom(
                            primary: AppColors.secondary,
                            fixedSize: Size(250, 40),
                        ),
                    ),

                ),

            ),

        );

        return WillPopScope(
            onWillPop: () async { exit(0); },
            child: bTasksLoaded ? appWidget : loadingWidget,
        );
    }

    bool bTasksLoaded = false;
}
