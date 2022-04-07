import 'package:flutter/material.dart';
import 'package:todolist/framework/app_colors.dart';
import 'package:todolist/framework/app_text.dart';
import 'package:todolist/framework/helpers.dart';
import 'package:todolist/framework/user_task.dart';

class TaskEntryWidget extends StatefulWidget {

    TaskEntryWidget({
        required this.taskId,
        required this.task,
        required this.onStateChangedCb,
        required this.onEditCb,
        required this.onRemoveCb
    });

    @override
    _TaskEntryWidgetState createState() => _TaskEntryWidgetState();

    int taskId;
    UserTask task;
    void Function(int, bool) onStateChangedCb;
    void Function(int) onEditCb;
    void Function(int) onRemoveCb;
}

class _TaskEntryWidgetState extends State<TaskEntryWidget> {

    @override
    Widget build(BuildContext context) {
        DateTime? date = widget.task.notificationTime;

        return Row(
            children: [
                Wrap(
                    spacing: 10,
                    children: [
                        Transform.scale(
                            scale: 1.25,
                            child: Checkbox(
                                value: widget.task.bIsDone,
                                onChanged: (state) {
                                    setState(() {
                                        widget.onStateChangedCb(
                                            widget.taskId,
                                            state!
                                        );
                                    });
                                },
                                shape: CircleBorder(),
                                activeColor: AppColors.primary,
                                splashRadius: 15,
                            ),
                        ),
                        Column(
                            children: [
                                SizedBox(
                                    width: 200,
                                    child: Text(
                                        widget.task.name,
                                        style: TextStyle(
                                            color: AppColors.textCommon,
                                            fontSize: 18.0,
                                        ),
                                        overflow: TextOverflow.ellipsis,
                                        maxLines: 1,
                                    ),
                                ),
                                SizedBox(
                                    height: 5,
                                ),
                                Text(
                                    AppText.labelNotification + " " + (
                                        (date != null)
                                            ? Helpers.dateToString(date)
                                            : AppText.labelNotifyNull
                                    ),
                                    style: TextStyle(
                                        color: AppColors.textHints,
                                        fontSize: 12.0,
                                    ),
                                ),
                            ],
                            crossAxisAlignment: CrossAxisAlignment.start,
                        ),
                    ],
                    alignment: WrapAlignment.start,
                    crossAxisAlignment: WrapCrossAlignment.center,
                ),
                Wrap(
                    spacing: 0,
                    children: [
                        Material(
                            color: Colors.transparent,
                            child: IconButton(
                                onPressed: (){
                                    widget.onEditCb(widget.taskId);
                                },
                                icon: Icon(
                                    Icons.edit,
                                    color: AppColors.primary,
                                ),
                                splashRadius: 20,
                            ),
                        ),
                        Material(
                            color: Colors.transparent,
                            child: IconButton(
                                onPressed: (){
                                    widget.onRemoveCb(widget.taskId);
                                },
                                icon: Icon(
                                    Icons.clear,
                                    color: AppColors.primary,
                                ),
                                splashRadius: 20,
                            ),
                        ),
                    ],
                    alignment: WrapAlignment.end,
                    crossAxisAlignment: WrapCrossAlignment.center,
                ),
            ],
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
        );
    }
}
