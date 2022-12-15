function showAlert(vue, namespace, title, msg){
    vue.$nextTick(function(){
        vue.$emit(namespace, title, msg);
    });

    vue.alertVisible = true;
}

export { showAlert }