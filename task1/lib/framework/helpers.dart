class Helpers {

    static String dateToString(DateTime date) {
        return  "${date.day}".padLeft(2, '0') + "/"
                    + "${date.month}".padLeft(2, '0')
                    + "/${date.year} "
                    + "${date.hour}".padLeft(2, '0') + ":"
                    + "${date.minute}".padLeft(2, '0');
    }
}