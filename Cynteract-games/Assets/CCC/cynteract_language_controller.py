from googletrans import Translator,constants
from pprint import pprint
translator=Translator()
execute=str(input("What do you want to do: Translate the whole document-Whole document || Translate only expression-Expression"))
if execute=="Whole document":
    #for lang in constants.LANGUAGES:
    prev_language=str(input("Enter edited Language start range: "))
    current_language=""
    file=open("Lean Localization_auto.prefab","r")
    entry=0;
    delete=0;
    delete_confirmation=0;
    line_sum=0;
    new_content="";
    get_expression=0
    expression=""

    put_out=0;
    def test_for_existance(line):
        if ("---!u!" in line) or ("GameObject" in line) or (("--- !u!") in line):
            return 0
        return 1

    def translate_expression_and_write(expression):
        result="";
        for lang in constants.LANGUAGES:
            translation=translator.translate(expression,dest=lang,src="en")
            current_language=constants.LANGUAGES[lang];
            new_expression=translation.text.replace(">>>>","\n").replace("@@@@","\r");
            new_expression=new_expression.replace('"\n',"\n").replace('\t"',"\t").replace(' "'," ").replace('" '," ")
            language_str="  - "+"Language: "+current_language.capitalize()+"\n";
            text_str='    Text: "'+new_expression.lstrip().replace('"',"'").encode("unicode-escape").decode("utf-8")+'"\n';
            object_str='    Object: {fileID: 0}\n';
            print(new_expression);
            result+=language_str+text_str+object_str;
        print("\n\n\n\n\n\n")
        return result;

    for i in file:
        line=i.replace("  "," ")
        line=line.replace(": ",":")

        if "- Language:English" in line:
            get_expression=1;

        if get_expression==1 and ("Text:" in line):
            get_expression=2;
            #new_content+=i;
            expression+=line.replace("Text:","").replace("\n",">>>>").replace("\r","@@@@")
            continue

        if get_expression==2 and ("Object:" in line):
            get_expression=0;
            
            
        if get_expression==2:
            expression+=line.replace("\n",">>>>").replace("\r","@@@@");

            
        if "entries:" in line:
            entry=1
            delete=0
            delete_confirmation=0
            
            
        if ("- Language:"+prev_language in line) and entry==1:
            delete=1;
            entry=0;
            delete_confirmation=0
            
        if delete==1 and ("Object:" in line):
            delete_confirmation=1;
            entry=0
            delete=0
            #new_content+=i
            continue

        if delete_confirmation:
            if test_for_existance(line):
                continue
            else:
                new_text=translate_expression_and_write(expression)
                expression=""
                new_content+=new_text
                delete_confirmation=0;
                delete=0;
                entry=0
                
        if ("---!u!" in line) or (("--- !u!") in line):
            put_out=0;
            
        if put_out==0:
            new_content+=i;
            
        if "entries:" in line:
            put_out=1;
    file.close()
    file=open("Lean Localization.prefab","w")
    file.write(new_content)
    file.close()
elif execute=="Expression":
    result=""
    expression=str(input("Enter expression: "))
    for lang in constants.LANGUAGES:
        translation=translator.translate(expression,dest=lang,src="en")
        current_language=constants.LANGUAGES[lang];
        new_expression=translation.text.replace(">>>>","\n").replace("@@@@","\r");
        new_expression=new_expression.replace('"\n',"\n").replace('\t"',"\t").replace(' "'," ").replace('" '," ")
        language_str="  - "+"Language: "+current_language.capitalize()+"\n";
        text_str='    Text: "'+new_expression.lstrip().replace('"',"'").encode("unicode-escape").decode("utf-8")+'"\n';
        object_str='    Object: {fileID: 0}\n';
        result+=language_str+text_str+object_str;
    print(result)
