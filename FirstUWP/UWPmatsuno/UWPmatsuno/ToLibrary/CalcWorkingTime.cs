using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPmatsuno.ToLibrary
{
    /// <summary>
    /// ２つのDateTime型、もしくはひとつのTimeSpan型から労働時間、深夜労働時間、
    /// 休憩時間などの計算を行う機能を提供するクラス。
    /// このクラスの出社時間、退社時間を表すDateTime型の引数は、時間、分の部分のみを使用することを
    /// 前提としており、呼び出し元が引数を渡す際に、年月日に関しては、今日の日付、秒に関しては
    /// 00を設定しておくことを前提に
    /// </summary>
    public static class CalcWorkingTime
    {

        //TimeSpan型から休憩時間を差し引く
        //戻り値：差し引いた休憩時間（分）
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


        //出社時間と終業時間から深夜労働した時間を割り出す
        //戻り値:深夜労働した時間
        static public TimeSpan NightWork(DateTime dtStart, DateTime dtLeave)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0);
            DateTime fiveAM = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day,
                5, 0, 0);
            DateTime TenPM = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day,
                22, 0, 0);

            if (dtLeave.CompareTo(dtStart) > 0)
            {//労働が日をまたがない
                if ((dtStart.CompareTo(TenPM) >= 0)
                    || (dtLeave.CompareTo(fiveAM) <= 0))
                {//出勤時間が22時以降、または終業時間が朝5時以前のとき：全て深夜労働
                    return dtLeave - dtStart;
                }

                if (dtStart.CompareTo(fiveAM) < 0)
                {//出勤時間が朝5時よりも前なとき：朝5時以前を深夜労働時間に追加
                    ts = ts.Add(fiveAM - dtStart);
                }
                if (dtLeave.CompareTo(TenPM) > 0)
                {//終業時間が22時よりも後のとき：22時以降を深夜労働時間に追加
                    ts = ts.Add(dtLeave - TenPM);
                }
                return ts;
            }
            else
            {//労働が日をまたぐ

                TimeSpan twoHours = TimeSpan.FromHours(2);
                TimeSpan fiveHours = TimeSpan.FromHours(5);

                if (dtStart.CompareTo(fiveAM) < 0)
                {//出勤時間が朝5時よりも前なとき：朝5時以前と22時～24時を深夜労働時間に追加
                    ts = ts.Add(fiveAM - dtStart);
                    ts = ts.Add(twoHours);
                }else if(dtStart.CompareTo(TenPM) <= 0)
                {//出勤時間が朝5時以降、22時以前のとき：22時～24時を深夜労働時間に追加
                    ts = ts.Add(twoHours);
                }
                else
                {//出勤時間が22時よりも後のとき：出勤時間～24時を深夜労働時間に追加
                    DateTime twelveAM = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day,
                        0, 0, 0);
                    twelveAM = twelveAM.AddDays(1);
                    ts = ts.Add(twelveAM - dtStart);
            


                }


                if (dtLeave.CompareTo(fiveAM) < 0)
                {//終業時間が朝5時よりも前なとき：0時～終業時間を深夜労働時間に追加
                    DateTime zeroAM = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day,
                        0, 0, 0);
                    ts = ts.Add(dtLeave - zeroAM);
                }
                else if (dtLeave.CompareTo(TenPM) <= 0)
                {//終業時間が朝5時以降、22時以前のとき：0時～5時を深夜労働時間に追加
                    ts = ts.Add(fiveHours);
                }
                else
                {//終業時間が22時よりも後のとき：0時～5時と22時～終業時間を深夜労働時間に追加
                    ts = ts.Add(fiveHours);
                    ts = ts.Add(dtLeave - TenPM);
                }

                return ts;
            }
            
        }//NightWorkメソッド



    }//CalcWorkingTimeクラス







}
