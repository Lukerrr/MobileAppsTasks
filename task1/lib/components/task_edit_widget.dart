import 'package:flutter/material.dart';
import 'package:todolist/components/text_field_widget.dart';
import 'package:todolist/framework/app_colors.dart';
import 'package:todolist/framework/app_text.dart';
import 'package:todolist/framework/helpers.dart';

class TaskEditWidgetController extends ChangeNotifier {

    void setName(String textName) {
        this.textName = textName;
        notifyListeners();
    }

    void setNotification(DateTime? dateNotification) {
        this.dateNotification = dateNotification;
        notifyListeners();
    }

    bool hasValidData() {
        return textName.isNotEmpty;
    }

    String textName = "";
    DateTime? dateNotification = null;
}

class TaskEditWidget extends StatefulWidget {

    TaskEditWidget({required this.controller}) {
        textFieldNameController.setText(controller.textName);

        textFieldNameController.addListener(() {
            controller.setName(textFieldNameController.text);
            controller.notifyListeners();
        });
    }

    @override
    _TaskEditWidgetState createState() => _TaskEditWidgetState();

    TextFieldWidgetController textFieldNameController = TextFieldWidgetController();

    TaskEditWidgetController controller;
}

class _TaskEditWidgetState extends State<TaskEditWidget> {

    void _pickDateTime() async {
        var currentDate = DateTime.now();

        DateTime? pickedDate = await showDatePicker(
            context: context,
            initialDate: currentDate,
            firstDate: currentDate,
            lastDate: DateTime(currentDate.year + 10),
            builder: (BuildContext context, Widget? child) {
                return Theme(
                    data: ThemeData.light().copyWith(
                        primaryColor: AppColors.primary,
                        colorScheme: ColorScheme.light(
                            primary: AppColors.primary,
                        ),
                        buttonTheme: ButtonThemeData(
                            textTheme: ButtonTextTheme.primary
                        ),
                    ),
                    child: child ?? Text(""),
                );
            },
        );

        if (pickedDate == null) {
            return;
        }

        TimeOfDay? pickedTime = await showTimePicker(
            context: context,
            initialTime: TimeOfDay.now(),
            builder: (BuildContext context, Widget? child) {
                return Theme(
                    data: ThemeData.light().copyWith(
                        primaryColor: AppColors.primary,
                        colorScheme: ColorScheme.light(
                            primary: AppColors.primary,
                        ),
                        buttonTheme: ButtonThemeData(
                            textTheme: ButtonTextTheme.primary
                        ),
                    ),
                    child: child ?? Text(""),
                );
            },
        );

        if (pickedTime == null) {
            return;
        }

        setState(() {
            widget.controller.setNotification(
                DateTime(
                    pickedDate.year,
                    pickedDate.month,
                    pickedDate.day,
                    pickedTime.hour,
                    pickedTime.minute,
                )
            );
        });
    }

    @override
    Widget build(BuildContext context) {
        DateTime? date = widget.controller.dateNotification;

        return Padding(
            padding: EdgeInsets.symmetric(
                horizontal: 30.0,
                vertical: 30.0,
            ),
            child: Wrap(
                runSpacing: 40,
                children: [
                    TextFieldWidget(
                        controller: widget.textFieldNameController,
                        labelText: AppText.labelInputTaskName,
                    ),
                    Wrap(
                        runSpacing: 3,
                        children: [
                            Text(
                                AppText.labelNotification,
                                style: TextStyle(
                                    color: AppColors.textCommon,
                                    fontSize: 16.0,
                                ),
                            ),
                            Row(
                                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                children: [
                                    Text(
                                        (date != null)
                                            ? Helpers.dateToString(date)
                                            : AppText.labelNotifyNull,
                                        style: TextStyle(
                                            color: AppColors.textHints,
                                            fontSize: 16.0,
                                        ),
                                    ),
                                    IconButton(
                                        onPressed: (){
                                            if (date != null) {
                                                widget.controller.setNotification(null);
                                            } else {
                                                _pickDateTime();
                                            }
                                        },
                                        icon: Icon(
                                            (date == null)
                                                ? Icons.access_time
                                                : Icons.clear,
                                            color: AppColors.primary,
                                        ),
                                    ),
                                ],
                            ),
                        ],
                    ),
                ],
            ),
        );
    }
}
