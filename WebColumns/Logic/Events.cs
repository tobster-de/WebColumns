﻿using System.Collections.Generic;

namespace WebColumns.Logic
{
    public delegate void NewPreviewAvailable(Triple triple);

    public delegate void ScoreChanged(int score, int elements, int level);

    public delegate void ElementsAdded(List<Element> elements);

    public delegate void ElementsRemoved(List<Element> elements);
}