using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialportDataAnalyzer
{
    class ComponentAnalyzer
    {
        private const string JDQ8ReturnStr = "020F00000008543E";   //8路继电器返回指令
        private const string JDQ32ReturnStr = "010F000000205413";    //32路继电器返回指令
        private int initComponentId = 6; //初始组件为6
        private int initAzimuth = -10;  //组件6初始方位角
        private int initObliquity = 22;  //组件6初始倾角
        private string[] componentSendStr = {    //测组件1-5的8路继电器指令
                    "020F0000000801017F40",
                    "020F0000000801023F41",
                    "020F000000080104BF43",
                    "020F000000080108BF46",
                    "020F000000080110BF4C"
                    };
        private int[,] componentAO = { { 0, 3 }, { 0, 22 }, { 0, 27 }, { 0, 32 }, { 0, 37 } }; //组件1-5的方位角和倾角

        private string[] component6SendStr = { "020F000000080140BF70",   //测组件6的8路继电器指令
                                                "020F000000080120BF58" };

        private string[] obliquityDec = { "010F0000002004040400008479",    //组件6倾角减少5度指令
                                         "010F000000200408080000472A" };
        private string[] obliquityInc = { "010F00000020040101000094B4",    //组件6倾角增加5度指令
                                            "010F00000020040202000064F0" };

        private string[] azimuthDec = { "010F0000002004000002024429", //组件6方位角增加5度指令
                                          "010F00000020040000010104D8" };
        private string[] azimuthInc = { "010F000000200400000404C78B",   //组件6方位角减少5度指令
                                          "010F000000200400000808C28E"};

        private List<KeyValuePair<DateTime, string>> sendInfo;

        public ComponentAnalyzer(List<KeyValuePair<DateTime, string>> sendInfo)
        {
            this.sendInfo = sendInfo;
        }

        /// <summary>
        /// 组件解析
        /// </summary>
        /// <param name="receiveTime">数据接收时间</param>
        /// <param name="componentId">组件号</param>
        /// <param name="azimuth">方位角</param>
        /// <param name="obliquity">倾角</param>
        public void Analy(DateTime receiveTime,List<KeyValuePair<byte, bool>> messageQueue,out int componentId, out int azimuth, out int obliquity)
        {
            JDQ8Analy(messageQueue);                             //8路解析
            JDQ32Analy(messageQueue);                            //32路解析

            int index = SearchMatches(receiveTime);
            string sendStr = sendInfo.ElementAt(index).Value;

            for (int i = 0; i < 5; i++)                           //1-5号组件
            {
                if (sendStr == componentSendStr[i])
                {
                    initComponentId = i + 1;
                    componentId = initComponentId;
                    azimuth = componentAO[i,0];
                    obliquity = componentAO[i, 1];
                    return;
                }
            }

            if (sendStr == component6SendStr[0] || sendStr == component6SendStr[1])   //如果是组件6的8路继电器指令
            {
                initComponentId = 6;
                componentId = initComponentId;
                azimuth = initAzimuth;
                obliquity = initObliquity;
                return;
            }

            if (sendStr == obliquityDec[0] || sendStr == obliquityDec[1])  //如果是组件6倾角减少5度指令
            {
                initComponentId = 6;
                componentId = initComponentId;
                azimuth = initAzimuth;
                initObliquity -= 5;
                obliquity = initObliquity;
                return;
            }

            if (sendStr == obliquityInc[0] || sendStr == obliquityInc[1])  //如果是组件6倾角增加5度指令
            {
                initComponentId = 6;
                componentId = initComponentId;
                azimuth = initAzimuth;
                initObliquity += 5;
                obliquity = initObliquity;
                return;
            }

            if (sendStr == azimuthDec[0] || sendStr == azimuthDec[1])    //如果是组件6方位角减少5度指令
            {
                initComponentId = 6;
                componentId = initComponentId;
                initAzimuth -= 5;
                azimuth = initAzimuth;
                obliquity = initObliquity;
                return;
            }

            if (sendStr == azimuthInc[0] || sendStr == azimuthInc[1])   //如果是组件6方位角增加5度指令
            {
                initComponentId = 6;
                componentId = initComponentId;
                initAzimuth += 5;
                azimuth = initAzimuth;
                obliquity = initObliquity;
                return;
            }
            componentId = initComponentId;
            azimuth = initAzimuth;
            obliquity = initObliquity;
        }

        /// <summary>
        /// 找到最接近所给时间的下标
        /// </summary>
        /// <param name="time">DateTime 参数</param>
        /// <returns></returns>
        private int SearchMatches(DateTime time)
        {
            int low = 0,high = sendInfo.Count() - 1;
            int mid;
            while (low + 1 < high)                    //二分查找
            {
                mid = (low + high) / 2;
                if (sendInfo.ElementAt(mid).Key.CompareTo(time) > 0)
                {
                    high = mid - 1;
                }
                else
                {
                    low = mid + 1;
                }
            }
            int t1,t2;
            t1 = Math.Abs(((TimeSpan)(time - sendInfo.ElementAt(low).Key)).Seconds);
            t2 = Math.Abs(((TimeSpan)(time - sendInfo.ElementAt(high).Key)).Seconds);
            if (t1 < t2)
                return low;
            else
                return high;
        }

        /// <summary>
        /// 8路继电器解析
        /// </summary>
        /// <param name="messageQueue">数据队列</param>
        private void JDQ8Analy(List<KeyValuePair<byte, bool>> messageQueue)
        {
            //!!!!!
            byte[] bytes = messageQueue.Select(b => b.Key).ToArray();

            String message = Transfer.BaToS(bytes);
            int index = -1;
            while ((index = message.IndexOf(JDQ8ReturnStr, index + 1)) != -1)
            {
                for (int i = index / 2; i < (index + JDQ8ReturnStr.Length) / 2; i++)
                    messageQueue[i] = new KeyValuePair<byte, bool>(messageQueue[i].Key, false);
            }

        }

        /// <summary>
        /// 32路继电器解析
        /// </summary>
        /// <param name="messageQueue">数据队列</param>
        private void JDQ32Analy(List<KeyValuePair<byte, bool>> messageQueue)
        {
            byte[] bytes = messageQueue.Select(b => b.Key).ToArray();
            String message = Transfer.BaToS(bytes);
            int index = -1;
            while ((index = message.IndexOf(JDQ32ReturnStr, index + 1)) != -1)
            {
                for (int i = index / 2; i < (index + JDQ32ReturnStr.Length) / 2; i++)
                    messageQueue[i] = new KeyValuePair<byte, bool>(messageQueue[i].Key, false);
            }
        }
    }
}
