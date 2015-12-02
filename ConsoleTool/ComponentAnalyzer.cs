using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialportDataAnalyzer
{
    class ComponentAnalyzer
    {
        private const string JDQ8ReturnStr = "020F00000008543E";
        private const string JDQ32ReturnStr = "010F000000205413";
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
        public void Analy(DateTime receiveTime,out int componentId, out int azimuth, out int obliquity)
        {
            ////////////////////没写完
            componentId = 0;
            azimuth = 0;
            obliquity = 0;
        }

        /// <summary>
        /// 8路继电器解析
        /// </summary>
        /// <param name="messageQueue">数据队列</param>
        public void JDQ8Analy(List<KeyValuePair<byte, bool>> messageQueue)
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
        public void JDQ32Analy(List<KeyValuePair<byte, bool>> messageQueue)
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
