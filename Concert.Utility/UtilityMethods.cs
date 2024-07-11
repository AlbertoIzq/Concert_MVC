using Stripe.Checkout;
using Concert.Models;
using Concert.Models.ViewModels;

namespace Concert.Utility
{
    public static class UtilityMethods
    {
        public static SessionCreateOptions SetStripeOptions(string successShortUrl, string cancelShortUrl,
            IEnumerable<OrderDetailSong> listSongs, IEnumerable<OrderDetailService> listServices)
        {
            string domain = "http://localhost:5251/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + successShortUrl,
                CancelUrl = domain + cancelShortUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            int songsQuantity = listSongs.Count();
            foreach (var setListSong in listSongs)
            {
                // Song
                var sessionLineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = 0,
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = setListSong.Song.Title,
                            Description = setListSong.Song.Artist
                        }
                    },
                    Quantity = 1,
                };
                options.LineItems.Add(sessionLineItem);
            }

            foreach (var setListService in listServices)
            {
                // Service fixed price
                var sessionLineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(setListService.Service.PriceFixed * 100), // 20,50€ => 2050
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = setListService.Service.Name + ", fixed price"
                        }
                    },
                    Quantity = 1,
                };
                options.LineItems.Add(sessionLineItem);

                // Service variable price (per song)
                sessionLineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(setListService.Service.PricePerSong * 100), // 20,50€ => 2050
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = setListService.Service.Name + ", price per song"
                        }
                    },
                    Quantity = songsQuantity,
                };
                options.LineItems.Add(sessionLineItem);
            }

            return options;
        }
    }
}
