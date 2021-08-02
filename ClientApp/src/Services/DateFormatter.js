export class DateFormatter {
    // Format is "yyyy-mm-dd"
    static dateToString(date: Date): string {
        let dd, mm, yyyy = "";
        if (date.getDate() < 10) {
            dd = "0" + date.getDate();
        }
        if (date.getMonth() + 1 < 10) {
            mm = "0" + (date.getMonth() + 1);
        }
        yyyy = date.getFullYear();
        
        return yyyy + "-" + mm + "-" + dd;
    }
}