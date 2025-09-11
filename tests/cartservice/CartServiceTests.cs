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
using System.Threading.Tasks;
using cartservice.test;
using Grpc.Core;
using Hipstershop;
using Xunit;

namespace cartservice
{
    public class CartServiceTests
    {
        [Fact]
        public async Task GetItem_NoAddItemBefore_EmptyCartReturned()
        {
            var cartStore = new MockCartStore();
            var cartService = new CartServiceImpl(cartStore);
            string userId = Guid.NewGuid().ToString();

            var request = new GetCartRequest
            {
                UserId = userId,
            };

            var cart = await cartService.GetCart(request, null);
            Assert.NotNull(cart);

            Assert.Equal(new Cart(), cart);
        }

        [Fact]
        public async Task AddItem_ItemExists_Updated()
        {
            var cartStore = new MockCartStore();
            var cartService = new CartServiceImpl(cartStore);
            string userId = Guid.NewGuid().ToString();

            var request = new AddItemRequest
            {
                UserId = userId,
                Item = new CartItem
                {
                    ProductId = "1",
                    Quantity = 1
                }
            };

            await cartService.AddItem(request, null);
            await cartService.AddItem(request, null);
            
            var getCartRequest = new GetCartRequest
            {
                UserId = userId
            };
            var cart = await cartService.GetCart(getCartRequest, null);
            Assert.NotNull(cart);
            Assert.Equal(userId, cart.UserId);
            Assert.Single(cart.Items);
            Assert.Equal(2, cart.Items[0].Quantity);

            await cartService.EmptyCart(new EmptyCartRequest{ UserId = userId }, null);
        }

        [Fact]
        public async Task AddItem_New_Inserted()
        {
            var cartStore = new MockCartStore();
            var cartService = new CartServiceImpl(cartStore);
            string userId = Guid.NewGuid().ToString();

            var request = new AddItemRequest
            {
                UserId = userId,
                Item = new CartItem
                {
                    ProductId = "1",
                    Quantity = 1
                }
            };

            await cartService.AddItem(request, null);

            var getCartRequest = new GetCartRequest
            {
                UserId = userId
            };
            var cart = await cartService.GetCart(getCartRequest, null);
            Assert.NotNull(cart);
            Assert.Equal(userId, cart.UserId);
            Assert.Single(cart.Items);

            await cartService.EmptyCart(new EmptyCartRequest{ UserId = userId }, null);
            cart = await cartService.GetCart(getCartRequest, null);
            Assert.Empty(cart.Items);
        }
    }
}
