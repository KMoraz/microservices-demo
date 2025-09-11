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

using System.Threading.Tasks;
using cartservice.interfaces;
using Grpc.Core;
using Hipstershop;

namespace cartservice
{
    // Cart wrapper to deal with grpc communication
    public class CartServiceImpl : CartService.CartServiceBase
    {
        private ICartStore cartStore;
        private readonly static Empty Empty = new Empty();

        public CartServiceImpl(ICartStore cartStore)
        {
            this.cartStore = cartStore;
        }

        public override async Task<Empty> AddItem(AddItemRequest request, ServerCallContext context)
        {
            await cartStore.AddItemAsync(request.UserId, request.Item.ProductId, request.Item.Quantity);
            return Empty;
        }

        public override async Task<Empty> EmptyCart(EmptyCartRequest request, ServerCallContext context)
        {
            await cartStore.EmptyCartAsync(request.UserId);
            return Empty;
        }

        public override Task<Cart> GetCart(GetCartRequest request, ServerCallContext context)
        {
            return cartStore.GetCartAsync(request.UserId);
        }
    }
}