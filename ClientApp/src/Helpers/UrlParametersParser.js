function getSearchParams(k){
    let p = {};
    window.location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi,function(s,k,v){p[k]=v})
    return k?p[k]:p;
}

export {getSearchParams}
