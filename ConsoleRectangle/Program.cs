using System;

namespace ConsoleRectangle
{
    public class SideData
    {
        // 转换为 L * 1 的矩形 rectLength∈（ 1 ， 100）
        public double rectLength;

        //最大边长
        public double maxSide;

        //最小边长
        public double minSide;

        //当最大中间值，大于零 ，直接取最大中间值，作为最终结果
        public double maxMiddleValue = -1;

        //最小边长下能排入的方块数量,需要大于100
        public int minSideCount
        {
            get => GetCount(minSide);
        }

        //最小边长下能排入的方块数量，通常情况该值小于100，当能排入100时，既找到最大近似值
        public int maxSideCount
        {
            get => GetCount(maxSide);
        }

        //将 sideValue 大小的 方块 塞入 t * 1 的矩形中能塞多少块
        public int GetCount(double side)
        {
            int column = (int)(rectLength / side);

            int row = (int)(1 / side);

            int count = row * column;

            Console.WriteLine(" side {0}  column {1}  row  {2}  count {3} ", side, column, row, count);

            return count;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            //使用100个大小相同的、彼此不重叠的正方形尽可能填满1个 M x N 的 矩形 区域 ， 求取正方形边长
            // M* N的矩形化简为 Y * 1(Y >= 1) 矩形 。
            // Y等于1 时 ， X = 0.1;
            // Y大于等于100时 ， X = 1;
            // 当前代码只计算Y值小于100大于1的情况  

            double rectLength = 3; 
            Console.WriteLine("矩形  {0} x 1", rectLength);

            SideData sideData;
            double sideValue = ComputeMaximum(rectLength, out sideData);

            Console.WriteLine("100正方形最大边长近似值 ： {0}   填充 {1}", sideValue, sideData.GetCount(sideValue));
        }

        public static double ComputeMaximum(double rectLength, out SideData sideData)
        {
            sideData = new SideData();
            sideData.rectLength = rectLength;

            //使用面积计算最大值 
            sideData.maxSide = Math.Sqrt(sideData.rectLength / 100);

            sideData.minSide = 0.1;
            
            while (sideData.maxMiddleValue < 0 && sideData.maxSideCount != 100 )
            {
                //Console.ReadKey();

                double diff = sideData.maxSide - sideData.minSide;

                //设定精确值 ， 精确值之后的值 ，不再计算
                if (diff < 0.000000000001)
                {
                    return sideData.minSide;
                }

                Console.WriteLine("NarrowRange  {0}  {1}  diff : {2} ", sideData.minSide, sideData.maxSide , diff);
                NarrowRange(sideData);
            }

            if (sideData.maxSideCount == 100)
            {
                return sideData.maxSide;
            }


            return sideData.maxMiddleValue;

        }


        //缩小 最大值 与 最小值 范围
        public static void NarrowRange(SideData sideData)
        {
            double middleValue = (sideData.minSide + sideData.maxSide) / 2;
            int middleValueCount = sideData.GetCount(middleValue);

            if (middleValueCount == 100)
            {
                //双边递归 ，缩小 最大最小值范围

                //中间值 与 最小值 递归 ,进一步缩减最小范围 ，如果成功缩减最小值范围， 重新计算一遍中间值 
                bool increasedMin = NarrowMinValue(middleValue, sideData, 0);
                if (increasedMin)
                {
                    return;
                }

                //当最小值，在有限递归层次下，无法进一步缩小范围时 ，
                //使用 中间值与最大值 递归 ，在有限递归层次下，如果成功缩小最大值范围 ， 重新计算中间值
                //在有限递归层次下 , 再无法进一步缩小最大值时， 将适合的最大中间值 作为 最大近似值
                bool reducedMax = NarrowMaxValue(middleValue, sideData, 0);
                if (reducedMax)
                {
                    return;
                }

                //在有限递归层次下，无法缩减了 ，直接返回最小值， 或者增加递归层次 
                if (sideData.maxMiddleValue < 0) sideData.maxMiddleValue = sideData.minSide;    

            }
            else if (middleValueCount > 100)
            {
                //中间值 替换 最小值
                sideData.minSide = middleValue;
                return;
            }
            else if (middleValueCount < 100)
            {
                //中间值 替换 最大值
                sideData.maxSide = middleValue;
                return;
            }

        }

        //通过中间值递归 ，逐渐增大最小值  
        public static bool NarrowMinValue(double middleValue, SideData sideData, int recursionCount)
        {
            recursionCount++;

            if (recursionCount > 6)
            {
                return false;
            }

            double toMiddleValue = (sideData.minSide + middleValue) / 2;
            int toMiddleCount = sideData.GetCount(toMiddleValue);
            if (toMiddleCount > 100)
            {
                //中间值 替换 最小值
                sideData.minSide = toMiddleValue;
                return true;
            }

            return NarrowMinValue(toMiddleValue, sideData, recursionCount);
        }

        //通过中间值递归 ，逐渐缩小最大值 
        public static bool NarrowMaxValue(double middleValue, SideData sideData, int recursionCount)
        {
            recursionCount++;

            double toMiddleValue = (sideData.maxSide + middleValue) / 2;
            int toMiddleCount = sideData.GetCount(toMiddleValue);

            if (recursionCount > 6)
            {
                if (toMiddleCount > 100)
                    sideData.maxMiddleValue = middleValue;
                else if (toMiddleCount == 100)
                    sideData.maxMiddleValue = toMiddleValue;

                return false;
            }

            if (toMiddleCount < 100)
            {
                //中间值 替换 最大值
                sideData.maxSide = toMiddleValue;
                return true;
            }

            return NarrowMaxValue(toMiddleValue, sideData, recursionCount);
        }

    }
}
