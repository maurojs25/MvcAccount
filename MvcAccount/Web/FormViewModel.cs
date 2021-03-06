﻿// Copyright 2012 Max Toro Q.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcAccount.Web {
   
   /// <summary>
   /// Holds common data for views that have a form.
   /// </summary>
   /// <typeparam name="TInputModel">The form's input type.</typeparam>
   public abstract class FormViewModel<TInputModel> : BaseViewModel {

      /// <summary>
      /// The form's input model.
      /// </summary>
      public TInputModel InputModel { get; private set; }

      /// <summary>
      /// Text for the submit button.
      /// </summary>
      public string SubmitText { get; set; }

      /// <summary>
      /// Initializes a new instance of the FormViewModel&lt;TInputModel> class
      /// using the provided input model.
      /// </summary>
      /// <param name="inputModel">The form's input model.</param>
      protected FormViewModel(TInputModel inputModel) {
         
         this.InputModel = inputModel;
         this.SubmitText = AccountResources.Views_Submit;
      }
   }
}
