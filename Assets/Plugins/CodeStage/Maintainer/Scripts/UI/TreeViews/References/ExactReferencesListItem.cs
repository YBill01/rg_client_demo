﻿#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using References;

	internal class ExactReferencesListItem<T> : MaintainerTreeViewItem<T> where T : ExactReferencesListElement
	{
		public ExactReferencesListItem(int id, int depth, string displayName, T data) : base(id, depth, displayName, data) { }

		protected override void Initialize()
		{
			if (depth == -1) return;
		}
	}
}