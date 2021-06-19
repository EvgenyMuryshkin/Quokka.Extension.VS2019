using Quokka.Extension.Interop;
using System;
using System.Threading.Tasks;

namespace Quokka.Extension.Examples
{
    [ExtensionClass]
    public class ExampleExtension
    {
        static async Task SimulateLongRunning(int duration)
        {
            for (int i = 0; i < duration; i++)
            {
                Console.WriteLine($"Iteration: {i}");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        [ExtensionMethod]
        public static async Task<int> LongRunning()
        {
            Console.WriteLine("VGATranslate started");
            await SimulateLongRunning(10);
            Console.WriteLine("VGATranslate completed");
            return 0;
        }

        [ExtensionMethod]
        public static async Task<int> Throws()
        {
            throw new NotImplementedException();
        }

        [ExtensionMethod(icon: AntDesignIcons.AiFillAndroid)]
        public static Task AiFillAndroidIcon()
        {
            Console.WriteLine("AntDesignIcons.AiFillAndroid clicked");
            return Task.CompletedTask;
        }

        [ExtensionMethod(icon: AntDesignIcons.AiFillApi)]
        public static Task AiFillApiIcon()
        {
            Console.WriteLine("AntDesignIcons.AiFillApi clicked");
            return Task.CompletedTask;
        }

        [ExtensionMethod(icon: TopLevelIcon.Translate, title: "Translate 1")]
        public static async Task TopMenuTranslate1()
        {
            Console.WriteLine("Translate 1");
            await SimulateLongRunning(5);
        }

        [ExtensionMethod(icon: TopLevelIcon.Translate, title: "Translate 2")]
        public static async Task TopMenuTranslate2()
        {
            Console.WriteLine("Translate 2");
            await SimulateLongRunning(5);
        }

        [ExtensionMethod(icon: TopLevelIcon.BitStream)]
        public static async Task TopMenuBitStream()
        {
            Console.WriteLine("BitStream");
            await SimulateLongRunning(5);
        }

        [ExtensionMethod(icon: TopLevelIcon.Program)]
        public static async Task TopMenuProgram()
        {
            Console.WriteLine("Program");
            await SimulateLongRunning(5);
        }
    }
}
