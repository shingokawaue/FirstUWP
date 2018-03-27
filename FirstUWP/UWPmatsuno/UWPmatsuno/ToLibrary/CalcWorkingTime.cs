using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPmatsuno.ToLibrary
{
    public static class CalcWorkingTime
    {

        //int型の値の先頭に0を付けたstring型を返す
        //戻り値　引いた分数
        public static int SubBreakTime(ref TimeSpan ts)
        {
            if (ts.TotalMinutes < 0)
            {
                throw new Exception("SubBreakTimeの引数が無効な値です");
            }

            TimeSpan ts2 = new TimeSpan(5, 0, 0);
            if (ts < ts2) return 0;//5時間未のとき：引かない


           ts2 = new TimeSpan(5, 30, 0);
            if (ts < ts2)//5時間以上５時間半未満のとき：５時間分を超えた分ひく
            {
                int minOver = ts.Minutes;
                ts = new TimeSpan(5, 0, 0);
                return minOver;
            }

            ts2 = new TimeSpan(10, 00, 0);
            if (ts < ts2)//５時間半以上１０時間未満のとき：３０分ひく
            {
                double totalmin;
                int hours, min;
                totalmin = ts.TotalMinutes - 30;//30分ひく
                hours = (int)totalmin / 60;
                min = (int)totalmin % 60;

                ts = new TimeSpan(hours, min , 0);
                return 30;
            }

            ts2 = new TimeSpan(10, 30, 0);
            if (ts < ts2)//10時間以上１０時間30分未満のとき：３０分+１０時間を超えた分だけひく
            {
                double totalmin;
                int hours, min , minOver;
                minOver = ts.Minutes;
                totalmin = ts.TotalMinutes - 30 - minOver;//30分と１０時間を超えた分ひく
                hours = (int)totalmin / 60;
                min = (int)totalmin % 60;

                ts = new TimeSpan(hours , min, 0);
                return 30 + minOver;
            }


//１０時間半以上のとき：１時間ひく
ts = new TimeSpan(ts.Hours - 1, ts.Minutes, 0);
            return 60;
        }

    }


}
