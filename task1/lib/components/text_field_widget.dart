import 'package:flutter/material.dart';

import 'package:todolist/framework/app_colors.dart';
import 'package:todolist/framework/app_text.dart';

class TextFieldWidgetController extends ChangeNotifier {

    void setText(String text) {
        this.text = text;
        notifyListeners();
    }

    String text = "";
}

class TextFieldWidget extends StatefulWidget {

    TextFieldWidget({required this.controller, this.labelText = ""});

    @override
    _TextFieldWidgetState createState() => _TextFieldWidgetState();

    String labelText;

    TextFieldWidgetController controller;
}

class _TextFieldWidgetState extends State<TextFieldWidget> {

    void _onTap(){
        setState(() {
            FocusScope.of(context).requestFocus(_focusNode);
        });
    }

    void _onChanged(String text) {
        setState(() {
            _bHasValidText = text.isNotEmpty;
            widget.controller.setText(text);
        });
    }

    @override
    void initState() {
        super.initState();

        controller.value = TextEditingValue(
            text: widget.controller.text,
            selection: TextSelection.fromPosition(
                TextPosition(
                    offset: controller.text.length
                )
            ),
        );
    }

    @override
    Widget build(BuildContext context) {
        final OutlineInputBorder border = OutlineInputBorder(
            borderRadius: BorderRadius.circular(10.0),
            borderSide: const BorderSide(
                color: AppColors.borders,
                width: 2.0,
            ),
            gapPadding: 2.0,
        );

        return TextField(
            focusNode: _focusNode,
            onTap: _onTap,
            onChanged: _onChanged,
            controller: controller,
            decoration: InputDecoration(
                enabledBorder: border,
                focusedBorder: border,
                errorBorder: border,
                focusedErrorBorder: border,
                labelText: widget.labelText,
                labelStyle: TextStyle(
                    color: _focusNode.hasFocus ? AppColors.textLabels : AppColors.textHints
                ),
                errorText: _bHasValidText ? null : AppText.labelInputInvalid,
            ),
        );
    }

    TextEditingController controller = TextEditingController();
    FocusNode _focusNode = FocusNode();
    bool _bHasValidText = true;
}
