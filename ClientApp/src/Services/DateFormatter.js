export class DateFormatter {
    // Format is "yyyy-mm-dd"
    static dateToString(date: Date): string {
        let dd, MM, yyyy, HH, mm, ss, SSS = "";
        if (date.getDate() < 10) {
            dd = "0" + date.getDate();
        }
        else {
            dd = date.getDate();
        }
        
        if (date.getMonth() + 1 < 10) {
            MM = "0" + (date.getMonth() + 1);
        }
        else {
            MM = (date.getMonth() + 1);
        }

        if (date.getHours() < 10) {
            HH = "0" + date.getHours();
        }
        else {
            HH = date.getHours();
        }

        if (date.getMinutes() < 10) {
            mm = "0" + date.getMinutes();
        }
        else {
            mm = date.getMinutes();
        }

        if (date.getSeconds() < 10) {
            ss = "0" + date.getSeconds();
        }
        else {
            ss = date.getSeconds();
        }
        
        yyyy = date.getFullYear();
        
        return yyyy + "-" + MM + "-" + dd + "T" + HH + ":" + mm + ":" + ss + ".000Z";
    }
}