﻿// Type definitions for Bootstrap Multiselect
// Project: https://github.com/davidstutz/bootstrap-multiselect
// Definitions by: Andrei Nicholson <https://github.com/tetsuo13/>

/// <reference path="../jquery/jquery.d.ts"/>

/**
* @see https://davidstutz.github.io/bootstrap-multiselect/#configuration-options
*/
interface MultiselectOptions {
    multiple ?: boolean;
    enableHTML ?: boolean;
    enableClickableOptGroups ?: boolean;
    disableIfEmpty ?: boolean;
    dropRight ?: boolean;
    maxHeight ?: number;
    checkboxName ?: string;
    onChange ?: any;
    onDropdownShow ?: any;
    onDropdownHide ?: any;
    onDropdownShown ?: any;
    onDropdownHidden ?: any;
    buttonClass ?: string;
    inheritClass ?: boolean;
    buttonContainer ?: string;
    buttonWidth ?: string;
    buttonText ?: any;
    buttonTitle ?: any;
    nonSelectedText ?: string;
    nSelectedText ?: string;
    allSelectedText ?: string;
    numberDisplayed ?: number;
    optionLabel ?: any;
    selectedClass ?: string;
    includeSelectAllOption ?: boolean;
    selectAllText ?: string;
    selectAllValue ?: string;
    selectAllName ?: string;
    selectAllNumber ?: boolean;
    onSelectAll ?: any;
    enableFiltering ?: boolean;
    enableCaseInsensitiveFiltering ?: boolean;
    filterBehavior ?: string;
    filterPlaceholder ?: string;
}

interface JQuery {
    multiselect(): JQuery;
    multiselect(select: JQuery): JQuery;
    multiselect(select: JQuery, options: MultiselectOptions): JQuery;
}
