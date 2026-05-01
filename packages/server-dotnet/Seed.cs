using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static async Task InitializeAsync(ReviewSummarizerDbContext context)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Seed products if empty
        if (!await context.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Barista Pro Coffee Machine",
                    Description = "Semi-automatic espresso machine with built-in grinder and milk frother, designed for home baristas.",
                    Price = 649.99
                },
                new Product
                {
                    Name = "WaveSound ANC Headphones",
                    Description = "Over-ear wireless headphones with active noise cancellation and 40-hour battery life.",
                    Price = 299.95
                },
                new Product
                {
                    Name = "EnduroFlex Running Shoes",
                    Description = "Lightweight running shoes designed for long-distance comfort and road stability.",
                    Price = 139.5
                },
                new Product
                {
                    Name = "PulseTrack Smartwatch",
                    Description = "Fitness-focused smartwatch with heart-rate tracking, GPS, and sleep analysis.",
                    Price = 229.0
                },
                new Product
                {
                    Name = "TitanStrike Mechanical Keyboard",
                    Description = "Mechanical gaming keyboard with hot-swappable switches and customizable RGB lighting.",
                    Price = 179.99
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        // Seed reviews if empty
        if (!await context.Reviews.AnyAsync())
        {
            var reviews = new List<Review>
            {
                new Review
                {
                    ProductId = 1,
                    Author = "Anna Kowalski",
                    Rating = 5,
                    Content = "I have been using this coffee machine every morning for the past two months, and it has completely transformed my home coffee routine. The grinder produces a very consistent grind, and the espresso tastes comparable to what I get in specialty cafes. The steam wand takes some practice, but once mastered, it creates beautifully textured milk for lattes and cappuccinos.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-08 17:46:28.482"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 1,
                    Author = "Mark Jensen",
                    Rating = 4,
                    Content = "This machine offers excellent value for serious coffee enthusiasts. The build quality feels solid, and the temperature stability during extraction is impressive. My only minor complaint is that it can be a bit noisy early in the morning, but the quality of the coffee makes up for it.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-09 08:12:15.123"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 1,
                    Author = "Sophie Martin",
                    Rating = 5,
                    Content = "Worth every penny! The espresso quality is incredible, and the built-in grinder is a game changer. No more grinding beans separately. Highly recommend for any coffee lover.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-10 10:22:45.789"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 2,
                    Author = "James Rodriguez",
                    Rating = 5,
                    Content = "These headphones are absolutely fantastic! The noise cancellation is industry-leading, and the sound quality is pristine. I use them daily for work calls and music, and they never disappoint.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-08 14:33:22.456"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 2,
                    Author = "Lisa Chen",
                    Rating = 4,
                    Content = "Great headphones overall. The ANC works really well on flights and in loud environments. Battery life is exceptional. Only minor issue is they can feel a bit heavy after extended wear.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-09 16:45:10.234"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 2,
                    Author = "David Thompson",
                    Rating = 4,
                    Content = "Solid build quality and excellent sound. The active noise cancellation is phenomenal. Comfort could be slightly better, but overall a top-tier product.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-11 09:15:33.567"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 3,
                    Author = "Emily Watson",
                    Rating = 5,
                    Content = "These shoes are perfect for my marathon training! They're incredibly lightweight yet provide excellent support. The cushioning is responsive, and I've had zero blisters or discomfort.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-08 06:20:11.890"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 3,
                    Author = "Michael Brown",
                    Rating = 4,
                    Content = "Great running shoes. They feel natural and responsive. Took about a week to break them in, but now they're my go-to. Perfect for both short runs and longer distances.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-09 07:30:44.123"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 3,
                    Author = "Sarah Williams",
                    Rating = 5,
                    Content = "Finally found the perfect running shoe! Lightweight, comfortable, and durable. I've put 200+ miles on these and they still feel great. Definitely buying another pair.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-10 06:45:22.345"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 4,
                    Author = "Robert Garcia",
                    Rating = 5,
                    Content = "This smartwatch has transformed how I track my fitness. The heart rate monitoring is accurate, GPS is reliable, and sleep tracking gives me valuable insights. Battery lasts nearly 2 weeks!",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-08 12:05:33.678"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 4,
                    Author = "Jessica Lee",
                    Rating = 4,
                    Content = "Excellent smartwatch for the price. Great fitness tracking features and the display is crisp. Would be perfect if the app had more customization options.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-09 18:20:15.901"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 4,
                    Author = "Kevin White",
                    Rating = 4,
                    Content = "Very impressed with this watch. The fitness tracking is comprehensive, GPS works great for running, and the design looks modern. Comfort level is high even for all-day wear.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-11 11:35:08.234"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 5,
                    Author = "Alex Turner",
                    Rating = 5,
                    Content = "This keyboard is phenomenal for gaming! The mechanical switches are responsive, hot-swappable design is awesome, and the RGB lighting is gorgeous. Build quality is exceptional.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-08 19:40:22.567"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 5,
                    Author = "Nicole Davis",
                    Rating = 5,
                    Content = "Best keyboard I've ever owned. The mechanical switches feel incredible, customizable RGB adds style, and the build quality is top-notch. Highly recommended for gamers and typists alike.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-09 20:12:44.789"), DateTimeKind.Utc)
                },
                new Review
                {
                    ProductId = 5,
                    Author = "Christopher Hall",
                    Rating = 4,
                    Content = "Great mechanical keyboard with excellent switch quality. The customizable RGB is a nice touch. Only minor complaint is it's slightly heavier than expected, but overall fantastic quality.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2026-04-10 21:55:10.456"), DateTimeKind.Utc)
                }
            };

            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();
        }
    }
}
