// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;
using PromptsDialog = Microsoft.Bot.Builder.Dialogs;

namespace PhoebeBot
{
    public static class PromptStep
    {
        public const string GatherInfo = "gatherInfo";
        public const string NamePrompt = "namePrompt";
        public const string AgePrompt = "agePrompt";
    }

    public class Phoebe : IBot
    {
        private readonly DialogSet dialogs;

        private async Task NameValidator(ITurnContext context, TextResult result)
        {
            if (result.Value.Length <= 2)
            {
                result.Status = PromptStatus.NotRecognized;
                await context.SendActivity("Your name should be at least 2 characters long.");
            }
        }

        private async Task AgeValidator(ITurnContext context, NumberResult<int> result)
        {
            if (0 > result.Value || result.Value > 122)
            {
                result.Status = PromptStatus.NotRecognized;
                await context.SendActivity("Your age should be between 0 and 122.");
            }
        }

        private async Task AskNameStep(DialogContext dialogContext, object result, SkipStepFunction next)
        {
            await dialogContext.Prompt(PromptStep.NamePrompt, "What is your name?");
        }

        private async Task AskAgeStep(DialogContext dialogContext, object result, SkipStepFunction next)
        {
            var state = dialogContext.Context.GetConversationState<PhoebeState>();
            state.Name = (result as TextResult).Value;
            await dialogContext.Prompt(PromptStep.AgePrompt, "What is your age?");
        }

        private async Task GatherInfoStep(DialogContext dialogContext, object result, SkipStepFunction next)
        {
            var state = dialogContext.Context.GetConversationState<PhoebeState>();
            state.Age = (result as NumberResult<int>).Value;
            await dialogContext.Context.SendActivity($"Your name is {state.Name} and your age is {state.Age}");
            await dialogContext.End();
        }

        public Phoebe()
        {
            dialogs = new DialogSet();

            // Create prompt for name with string length validation
            dialogs.Add(PromptStep.NamePrompt,
                new PromptsDialog.TextPrompt(NameValidator));
            // Create prompt for age with number value validation
            dialogs.Add(PromptStep.AgePrompt,
                new PromptsDialog.NumberPrompt<int>(Culture.English, AgeValidator));
            // Add a dialog that uses both prompts to gather information from the user
            dialogs.Add(PromptStep.GatherInfo,
                new WaterfallStep[] { AskNameStep, AskAgeStep, GatherInfoStep });
        }

        public async Task OnTurn(ITurnContext context)
        {
            var state = context.GetConversationState<PhoebeState>();
            var dialogCtx = dialogs.CreateContext(context, state);
            switch (context.Activity.Type)
            {
                case ActivityTypes.Message:
                    await dialogCtx.Continue();
                    if (!context.Responded)
                    {
                        await dialogCtx.Begin(PromptStep.GatherInfo);
                    }
                    break;
            }
        }
    }
}
//using System.Threading.Tasks;
//using Microsoft.Bot;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.Core.Extensions;
//using Microsoft.Bot.Schema;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.Prompts;
//using System.Linq;
//using System.Collections.Generic;
//using Microsoft.Recognizers.Text;

//namespace PhoebeBot
//{
//    public class Phoebe : IBot
//    {
//        //private DialogSet _dialogs;
//        //public EchoBot()
//        //{
//        //    _dialogs = new DialogSet();
//        //    _dialogs.Add("addTwoNumbers", new WaterfallStep[]
//        //    {
//        //    async (dc, args, next) =>
//        //    {
//        //        double sum = (double)args["first"] + (double)args["second"];
//        //        await dc.Context.SendActivity($"{args["first"]} + {args["second"]} = {sum}");
//        //        await dc.End();
//        //    }
//        //    });
//        //}
//        /// <summary>
//        /// Every Conversation turn for our EchoBot will call this method. In here
//        /// the bot checks the Activty type to verify it's a message, bumps the 
//        /// turn conversation 'Turn' count, and then echoes the users typing
//        /// back to them. 
//        /// </summary>
//        /// <param name="context">Turn scoped context containing all the data needed
//        /// for processing this conversation turn. </param>        
//        //public async Task OnTurn(ITurnContext context)
//        //{


//        //    // This bot is only handling Messages
//        //    if (context.Activity.Type == ActivityTypes.Message)
//        //    {
//        //        // Get the conversation state from the turn context
//        //        var state = context.GetConversationState<EchoState>();


//        //        var activity = context.Activity.Text;
//        //        var myAnswer = "";

//        //        var message = MessageFactory.Attachment( new Attachment[]{
//        //            new Attachment{ContentUrl= "C:/Users/waalmond/Pictures/CartScan/M0Peak.png", ContentType="image/png" },
//        //            new Attachment{ContentUrl= "C:/Users/waalmond/Pictures/Screenshots/RR/RR_Associate Vest.png", ContentType="image/png" }
//        //        });

//        //        // Bump the turn count. 
//        //        state.TurnCount++;

//        //        if (activity != "hi")
//        //        {
//        //            myAnswer = $"{activity} yourself";
//        //            await context.SendActivity(myAnswer);
//        //        } else
//        //        {

//        //        }


//        //        // Echo back to the user whatever they typed.
//        //        //await context.SendActivity(myAnswer);
//        //        //await context.SendActivity($"{context.Activity.Text}");
//        //    }
//        //}
//        //public async Task OnTurn(ITurnContext context)
//        //{
//        //    var state = ConversationState<Dictionary<string, object>>.Get(context);
//        //    var prompt = new TextPrompt();
//        //    var options = new PromptOptions { PromptString = "Hello, I'm the demo bot. What is your name?" };

//        //    switch (context.Activity.Type)
//        //    {
//        //        case ActivityTypes.ConversationUpdate:
//        //            if (context.Activity.MembersAdded[0].Id != context.Activity.Recipient.Id)
//        //            {
//        //                await prompt.Begin(context, state, options);
//        //            }
//        //            break;
//        //        case ActivityTypes.Message:
//        //            var dialogCompletion = await prompt.Continue(context, state);
//        //            if (!dialogCompletion.IsActive && !dialogCompletion.IsCompleted)
//        //            {
//        //                await prompt.Begin(context, state, options);
//        //            }
//        //            else if (dialogCompletion.IsCompleted)
//        //            {
//        //                var textResult = (Microsoft.Bot.Builder.Prompts.TextResult)dialogCompletion.Result;
//        //                await context.SendActivity($"'{textResult.Value}' is a great name!");
//        //            }
//        //            break;
//        //    }
//        //}
//        //DialogSet dialog = new DialogSet();
//        public async Task OnTurn(ITurnContext turnContext)
//        {
//            //dialog.Add("textPrompt", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
//            //dialog.Add("greetings", new WaterfallStep[]
//            //{
//            //    // Each step takes in a dialog context, arguments, and the next delegate.
//            //    async (dc, args, next) =>
//            //    {
//            //        // Prompt for the guest's name.
//            //        await dc.Prompt("textPrompt","What is your name?");
//            //    },
//            //    async(dc, args, next) =>
//            //    {
//            //        await dc.Context.SendActivity($"Hi {args["Text"]}!");
//            //        await dc.End();
//            //    }
//            //});
//            var userName = "Friend";
//            var response = "";
//            var namePrompt = new Microsoft.Bot.Builder.Prompts.TextPrompt();
//            var state = turnContext.GetConversationState<EchoState>();
//            switch (turnContext.Activity.Type)
//            {
//                case ActivityTypes.ConversationUpdate:
//                    userName = turnContext.Activity.MembersAdded.FirstOrDefault()?.Name;
//                    if (!string.IsNullOrWhiteSpace(userName) && userName != "Bot")
//                    {
//                        await turnContext.SendActivity($"Hello {userName}!");
//                    }
//                    break;

//                case ActivityTypes.Message:
//                    if (!state.PromptinName)
//                    {
//                        state.PromptinName = true;
//                        await namePrompt.Prompt(turnContext, "What's your name?");

//                        var name = await namePrompt.Recognize(turnContext);
//                        if (name.Succeeded())
//                        {
//                            state.name = name.Value;

//                        }
//                    }

//                    if (state.name == "John")
//                    {
//                        response = "go home clown";
//                    }
//                    break;

//                default:
//                    await turnContext.SendActivity($"I love you");
//                    break;


//            }
//            if (!string.IsNullOrEmpty(state.name))
//            {
//                await turnContext.SendActivity("Your name is " + state.name);
//            }
//            //await Task.CompletedTask;
//        }

//        public void Greeting(ITurnContext cont)
//        {

//        }

//    }
//}
