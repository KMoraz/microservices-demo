// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cartservice.interfaces;
using Hipstershop;

namespace cartservice.test
{
    public class MockCartStore : ICartStore
    {
        private readonly Dictionary<string, Cart> carts = new Dictionary<string, Cart>();

        public Task AddItemAsync(string userId, string productId, int quantity)
        {
            if (!carts.ContainsKey(userId))
            {
                carts[userId] = new Cart { UserId = userId };
            }
            var cart = carts[userId];
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity;
            }
            return Task.CompletedTask;
        }

        public Task<Cart> GetCartAsync(string userId)
        {
            if (!carts.ContainsKey(userId))
            {
                return Task.FromResult(new Cart());
            }
            return Task.FromResult(carts[userId]);
        }

        public Task EmptyCartAsync(string userId)
        {
            if (carts.ContainsKey(userId))
            {
                carts.Remove(userId);
            }
            return Task.CompletedTask;
        }

        public bool Ping()
        {
            return true;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
