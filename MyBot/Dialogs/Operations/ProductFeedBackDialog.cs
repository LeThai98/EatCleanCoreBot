using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBot.Dialogs.Operations
{
    public class ProductFeedBackDialog: ComponentDialog
    {
        public ProductFeedBackDialog() : base(nameof(ProductFeedBackDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {

            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }
    }
}

