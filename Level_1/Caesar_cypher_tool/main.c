#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <ctype.h>

void build_index(char *AZ);
int main(int argc, char *argv[])
{
    int i, n, j, x;
    char text[1000]; // text input
    char cypher[1000]; // cypher output
    char AZ[26]; // alphabetical indexes


    // in case if user didn't input proper console argument
    if (!(argc == 2) || (strlen(argv[1]) != 26))
    {
        printf("Usage: ./substitution key");
        return 1;
    }

    // build A-Z and a-z indexes
    build_index(AZ);

    // check console argument for wrong or non-unique characters
    for (i = 0; i < 26; i++)
    {
        // capitalise letters
        if (argv[1][i] >= 'a' && argv[1][i] <= 'z')
            argv[1][i] = toupper(argv[1][i]);

        // take capital letter and compare it with each letter in index
        if (argv[1][i] >= 'A' && argv[1][i] <= 'Z')
        {
            for (j = 0; j < 26; j++)
            {
                // if letter found in indexes - make this index variable 0
                if (argv[1][i] == AZ[j])
                    AZ[j] = 0;
            }
        }
        else
        {
            printf("Error: entered wrong symbols as a key");
            return 1;
        }
    }

    // finally argv processing step: summ all indexes
    for (i = 0, j = 0; j < 26; j++)
        i += (int)AZ[j];
    // summ should be 0
    if (i > 0)
    {
        printf("Error: key must consist out of unique characters");
        return 1;
    }


    // now lets get phrase to cipher
    printf("plaintext: ");

    for (i = 0, n = 0; ; i++)
    {
        scanf ("%c", &text[i]);        // get user input
        if (text[i] == '\n')
        {
            printf("\n");
            break;    // end input when user press 'Enter'
        }
        n++; // count number of input
    }


    // now lets get phrase to cipher
    printf("ciphertext: ");

    // replace plaintext with cipher
    for (i = 0; i <= n; i++)
    {
        if (text[i] >= 'A' && text[i] <= 'Z')
        {
            x = (int)text[i] - 65;
            printf("%c", argv[1][x]);
        }
        else if (text[i] >= 'a' && text[i] <= 'z')
        {
            x = (int)text[i] - 97;
            printf("%c", tolower(argv[1][x]));
        }
        else
            printf("%c", text[i]);
    }

    getchar();
    getchar();
    return 0;
}

void build_index(char *AZ)
{
    int i, j;
    for (i = 0, j = 65; i < 26; i++)
        AZ[i] = j++;
}
